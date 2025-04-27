namespace ForestNET.Lib.Net.Mail
{
    /// <summary>
    /// Encapsulation of an email message with all important information such as from, to, cc, bcc, subject, content, timing, attachments, etc.
    /// </summary>
    public class Message
    {

        /* Constants */

        public const string FIlENAMESEPARATOR = "%_-_%";
        public static readonly string[] FLAGS = { "ANSWERED", "DELETED", "DRAFT", "FLAGGED", "RECENT", "SEEN", "USER" };

        /* Fields */

        private string? s_from;
        private readonly List<string> a_from;

        private string? s_to;
        private readonly List<string> a_to;
        private string? s_cc;
        private readonly List<string> a_cc;
        private string? s_bcc;
        private readonly List<string> a_bcc;

        private string? s_subject;
        private string? s_text;
        private string? s_html;

        private readonly List<string> a_attachments;
        private readonly List<byte[]> a_attachmentsContent;
        private readonly List<string> a_flags;

        private string? s_messageId;

        /* Properties */

        public string? From
        {
            get
            {
                return this.s_from;
            }
            set
            {
                if ((value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    this.a_from.Clear();

                    if (!ValidateAddress(value))
                    {
                        throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                    }

                    this.s_from = value;
                    this.a_from.Add(this.s_from);
                }
                else
                {
                    this.s_from = null;
                }
            }
        }
        public List<string> FromList
        {
            get
            {
                return this.a_from;
            }
            set
            {
                if ((value == null) || (value.Count < 1))
                {
                    throw new ArgumentException("Empty e-mail-address list parameter or more than just one list element");
                }
                else
                {
                    this.a_from.Clear();

                    foreach (string s_foo in value)
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }

                        this.a_from.Add(s_foo);
                    }
                }
            }
        }
        public string? To
        {
            get
            {
                return this.s_to;
            }
            set
            {
                if ((value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    this.a_to.Clear();

                    if (value.Contains(';'))
                    {
                        foreach (string s_foo in value.Split(';'))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_to.Add(s_foo);
                            }
                        }
                    }
                    else if (value.Contains(','))
                    {
                        foreach (string s_foo in value.Split(","))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_to.Add(s_foo);
                            }
                        }
                    }
                    else
                    {
                        if (!ValidateAddress(value))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                        }
                        else
                        {
                            this.a_to.Add(value);
                        }
                    }

                    this.s_to = value;
                }
                else
                {
                    throw new ArgumentException("Empty value for \"To\"-field of E-Mail-Message.");
                }
            }
        }
        public List<string> ToList
        {
            get
            {
                return this.a_to;
            }
            set
            {
                if ((value == null) || (value.Count < 1))
                {
                    throw new ArgumentException("Empty e-mail-address list[TO] parameter");
                }
                else
                {
                    this.a_to.Clear();

                    foreach (string s_foo in value)
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }

                        this.a_to.Add(s_foo);
                    }
                }
            }
        }
        public string? CC
        {
            get
            {
                return this.s_cc;
            }
            set
            {
                if ((value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    this.a_cc.Clear();

                    if (value.Contains(';'))
                    {
                        foreach (string s_foo in value.Split(";"))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_cc.Add(s_foo);
                            }
                        }
                    }
                    else if (value.Contains(','))
                    {
                        foreach (string s_foo in value.Split(","))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_cc.Add(s_foo);
                            }
                        }
                    }
                    else
                    {
                        if (!ValidateAddress(value))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                        }
                        else
                        {
                            this.a_cc.Add(value);
                        }
                    }

                    this.s_cc = value;
                }
                else
                {
                    this.s_cc = null;
                }
            }
        }
        public List<string> CCList
        {
            get
            {
                return this.a_cc;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Empty e-mail-address list[CC] parameter");
                }
                else
                {
                    this.a_cc.Clear();

                    foreach (string s_foo in value)
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }

                        this.a_cc.Add(s_foo);
                    }
                }
            }
        }
        public string? BCC
        {
            get
            {
                return this.s_bcc;
            }
            set
            {
                if ((value != null) && (!ForestNET.Lib.Helper.IsStringEmpty(value)))
                {
                    this.a_bcc.Clear();

                    if (value.Contains(';'))
                    {
                        foreach (string s_foo in value.Split(";"))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_bcc.Add(s_foo);
                            }
                        }
                    }
                    else if (value.Contains(','))
                    {
                        foreach (string s_foo in value.Split(","))
                        {
                            if (!ValidateAddress(s_foo))
                            {
                                throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                            }
                            else
                            {
                                this.a_bcc.Add(s_foo);
                            }
                        }
                    }
                    else
                    {
                        if (!ValidateAddress(value))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                        }
                        else
                        {
                            this.a_bcc.Add(value);
                        }
                    }

                    this.s_bcc = value;
                }
                else
                {
                    this.s_bcc = null;
                }
            }
        }
        public List<string> BCCList
        {
            get
            {
                return this.a_bcc;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Empty e-mail-address list[BCC] parameter");
                }
                else
                {
                    this.a_bcc.Clear();

                    foreach (string s_foo in value)
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }

                        this.a_bcc.Add(s_foo);
                    }
                }
            }
        }
        public string? Subject
        {
            get
            {
                return this.s_subject;
            }
            set
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    this.s_subject = value;
                }
                else
                {
                    throw new ArgumentException("Empty value for \"Subject\"-field of E-Mail-Message");
                }
            }
        }
        public string? Text
        {
            get
            {
                return this.s_text;
            }
            set
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    this.s_text = value;
                }
                else
                {
                    throw new ArgumentException("Empty value for \"Text\"-field of E-Mail-Message");
                }
            }
        }
        public string? Html
        {
            get
            {
                return this.s_html;
            }
            set
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    this.s_html = value;
                }
            }
        }
        public DateTime? Send { get; set; } = null;
        public DateTime? Received { get; set; } = null;
        public string? ContentType { get; set; } = null;
        public bool Expunged { get; set; } = false;
        public string? Description { get; set; } = null;
        public string? Disposition { get; set; } = null;
        public int Size { get; set; } = -1;
        public List<string> Attachments
        {
            get
            {
                return this.a_attachments;
            }
            set
            {
                if (value != null)
                {
                    if (value.Count < 1)
                    {
                        throw new ArgumentException("Attachment-List has no entries");
                    }

                    this.a_attachments.Clear();

                    foreach (string s_foo in value)
                    {
                        this.a_attachments.Add(s_foo);
                    }
                }
            }
        }
        public bool HasAttachments
        {
            get
            {
                if ((this.a_attachmentsContent != null) && (this.a_attachmentsContent.Count > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public List<byte[]> AttachmentsContent
        {
            get
            {
                return this.a_attachmentsContent;
            }
            set
            {
                if (value != null)
                {
                    if (value.Count < 1)
                    {
                        throw new ArgumentException("AttachmentContent-List has no entries");
                    }

                    this.a_attachmentsContent.Clear();

                    foreach (byte[] a_foo in value)
                    {
                        if (a_foo.Length < 1)
                        {
                            throw new ArgumentException("Attachment has no bytes");
                        }
                        else
                        {
                            this.a_attachmentsContent.Add(a_foo);
                        }
                    }
                }
            }
        }
        public List<string> Flags
        {
            get
            {
                return this.a_flags;
            }
            set
            {
                if (value != null)
                {
                    if (value.Count < 1)
                    {
                        throw new ArgumentException("Attachment-List has no entries");
                    }

                    this.a_flags.Clear();

                    foreach (string s_foo in value)
                    {
                        if (!Message.FLAGS.Contains(s_foo))
                        {
                            throw new ArgumentException("Flag[" + s_foo + "] is invalid and cannot be attached to E-Mail-Message");
                        }
                        else
                        {
                            this.a_flags.Add(s_foo);
                        }
                    }
                }
            }
        }
        private List<string> Header { get; set; } = [];
        public string? MessageId
        {
            get
            {
                if (ForestNET.Lib.Helper.IsStringEmpty(this.s_messageId))
                {
                    return "NO_MESSAGE_ID";
                }
                else
                {
                    return this.s_messageId;
                }
            }
            set
            {
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("Empty value for message id");
                }
                else
                {
                    this.s_messageId = value;
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Create email message object, no source mail address, no cc, no bcc
        /// </summary>
        /// <param name="p_s_to">destination mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_subject">subject of message</param>
        /// <param name="p_s_text">text content of message</param>
        /// <exception cref="ArgumentException">parameters have invalid value or are empty</exception>
        public Message(string? p_s_to, string? p_s_subject, string? p_s_text) :
            this(null, p_s_to, null, null, p_s_subject, p_s_text)
        {

        }

        /// <summary>
        /// Create email message object, no source mail address, no bcc
        /// </summary>
        /// <param name="p_s_to">destination mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_cc">cc mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_subject">subject of message</param>
        /// <param name="p_s_text">text content of message</param>
        /// <exception cref="ArgumentException">parameters have invalid value or are empty</exception>
        public Message(string? p_s_to, string? p_s_cc, string? p_s_subject, string? p_s_text) :
            this(null, p_s_to, p_s_cc, null, p_s_subject, p_s_text)
        {

        }

        /// <summary>
        /// Create email message object, no source mail address
        /// </summary>
        /// <param name="p_s_to">destination mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_cc">cc mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_bcc">bcc mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_subject">subject of message</param>
        /// <param name="p_s_text">text content of message</param>
        /// <exception cref="ArgumentException">parameters have invalid value or are empty</exception>
        public Message(string? p_s_to, string? p_s_cc, string? p_s_bcc, string? p_s_subject, string? p_s_text) :
            this(null, p_s_to, p_s_cc, p_s_bcc, p_s_subject, p_s_text)
        {

        }

        /// <summary>
        /// Create email message object
        /// </summary>
        /// <param name="p_s_from">source mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_to">destination mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_cc">cc mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_bcc">bcc mail address of message (multiple addresses separated by ';')</param>
        /// <param name="p_s_subject">subject of message</param>
        /// <param name="p_s_text">text content of message</param>
        /// <exception cref="ArgumentException">parameters have invalid value or are empty</exception>
        public Message(string? p_s_from, string? p_s_to, string? p_s_cc, string? p_s_bcc, string? p_s_subject, string? p_s_text)
        {
            /* init list variables */
            this.a_from = [];
            this.a_to = [];
            this.a_cc = [];
            this.a_bcc = [];
            this.a_flags = [];
            this.a_attachments = [];
            this.a_attachmentsContent = [];

            /* adopt parameter values */
            this.From = p_s_from;
            this.To = p_s_to;
            this.CC = p_s_cc;
            this.BCC = p_s_bcc;
            this.Subject = p_s_subject;
            this.Text = p_s_text;

            /* set other variables to null */
            this.s_html = null;
            this.s_messageId = null;
        }

        /// <summary>
        /// Add mail address to To-List
        /// </summary>
        /// <param name="p_s_value">mail address value</param>
        /// <exception cref="ArgumentException">Invalid mail-address</exception>
        public void AddTo(string p_s_value)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_value))
            {
                if (p_s_value.Contains(';'))
                {
                    foreach (string s_foo in p_s_value.Split(";"))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.ToList.Add(s_foo);
                        }
                    }

                    this.To += ";" + p_s_value;
                }
                else if (p_s_value.Contains(','))
                {
                    foreach (string s_foo in p_s_value.Split(","))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.ToList.Add(s_foo);
                        }
                    }

                    this.To += "," + p_s_value;
                }
                else
                {
                    if (!ValidateAddress(p_s_value))
                    {
                        throw new ArgumentException("Invalid e-mail-address '" + p_s_value + "'");
                    }
                    else
                    {
                        this.ToList.Add(p_s_value);
                        this.To = p_s_value;
                    }
                }
            }
        }

        /// <summary>
        /// Add mail address to CC-List
        /// </summary>
        /// <param name="p_s_value">mail address value</param>
        /// <exception cref="ArgumentException">Invalid mail-address</exception>
        public void AddCC(string value)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(value))
            {
                if (value.Contains(';'))
                {
                    foreach (string s_foo in value.Split(";"))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.CCList.Add(s_foo);
                        }
                    }

                    this.CC += ";" + value;
                }
                else if (value.Contains(','))
                {
                    foreach (string s_foo in value.Split(","))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.CCList.Add(s_foo);
                        }
                    }

                    this.CC += "," + value;
                }
                else
                {
                    if (!ValidateAddress(value))
                    {
                        throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                    }
                    else
                    {
                        this.CCList.Add(value);
                        this.CC = value;
                    }
                }
            }
        }

        /// <summary>
        /// Add mail address to BCC-List
        /// </summary>
        /// <param name="p_s_value">mail address value</param>
        /// <exception cref="ArgumentException">Invalid mail-address</exception>
        public void AddBCC(string value)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(value))
            {
                if (value.Contains(';'))
                {
                    foreach (string s_foo in value.Split(";"))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.BCCList.Add(s_foo);
                        }
                    }

                    this.BCC += ";" + value;
                }
                else if (value.Contains(','))
                {
                    foreach (string s_foo in value.Split(","))
                    {
                        if (!ValidateAddress(s_foo))
                        {
                            throw new ArgumentException("Invalid e-mail-address '" + s_foo + "'");
                        }
                        else
                        {
                            this.BCCList.Add(s_foo);
                        }
                    }

                    this.BCC += "," + value;
                }
                else
                {
                    if (!ValidateAddress(value))
                    {
                        throw new ArgumentException("Invalid e-mail-address '" + value + "'");
                    }
                    else
                    {
                        this.BCCList.Add(value);
                        this.BCC = value;
                    }
                }
            }
        }

        /// <summary>
        /// Add file path to attachment list
        /// </summary>
        /// <param name="p_s_value">file path value</param>
        /// <exception cref="ArgumentException">empty value for adding attachment to mail-message</exception>
        public void AddAttachment(string p_s_filePath)
        {
            if (this.Attachments == null)
            {
                this.Attachments = [];
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_filePath))
            {
                throw new ArgumentException("Empty value for adding attachment to E-Mail-Message");
            }
            else
            {
                this.Attachments.Add(p_s_filePath);
            }
        }

        /// <summary>
        /// Add byte array to file attachment list
        /// </summary>
        /// <param name="p_a_bytes">byte array content of a file</param>
        /// <exception cref="ArgumentException">empty value for adding attachment bytes to mail-message</exception>
        public void AddAttachmentContent(byte[] p_a_bytes)
        {
            if (this.AttachmentsContent == null)
            {
                this.AttachmentsContent = [];
            }

            if ((p_a_bytes == null) || (p_a_bytes.Length < 0))
            {
                throw new ArgumentException("Empty value for adding attachment bytes to E-Mail-Message");
            }
            else
            {
                this.AttachmentsContent.Add(p_a_bytes);
            }
        }

        /// <summary>
        /// Add message flag
        /// </summary>
        /// <param name="p_s_flag">mail message as string</param>
        /// <exception cref="ArgumentException">empty value for adding flag to mail-message or invalid message flag constant</exception>
        public void AddFlag(string p_s_flag)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_flag))
            {
                throw new ArgumentException("Empty value for adding flag to E-Mail-Message");
            }
            else
            {
                if (!Message.FLAGS.Contains(p_s_flag))
                {
                    throw new ArgumentException("Flag[" + p_s_flag + "] is invalid and cannot be attached to E-Mail-Message");
                }

                this.Flags.Add(p_s_flag);
            }
        }

        public void AddHeaderLine(string value)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(value))
            {
                throw new ArgumentException("Empty value for adding header line to E-Mail-Message");
            }
            else
            {
                this.Header.Add(value);
            }
        }

        /// <summary>
        /// Checks if a mail address is valid or not
        /// </summary>
        /// <param name="p_s_address">mail address value, e.g. test@host.com</param>
        /// <returns>true - valid, false - not valid</returns>
        private static bool ValidateAddress(string? p_s_address)
        {
            try
            {
                _ = new MimeKit.MailboxAddress(null, p_s_address);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a multiline string with all information of the mail message object, no additional and no header information
        /// </summary>
        /// <returns>string</returns>
        override public string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// Returns a multiline string with all information of the mail message object, no header information
        /// </summary>
        /// <param name="p_b_extended">return additional information like bcc, content type, size or message id</param>
        /// <returns>string</returns>
        public string ToString(bool p_b_extended)
        {
            return this.ToString(p_b_extended, false);
        }

        /// <summary>
        /// Returns a multiline string with all information of the mail message object
        /// </summary>
        /// <param name="p_b_extended">return additional information like bcc, content type, size or message id</param>
        /// <param name="p_b_returnHeader">return header information lines</param>
        /// <returns>string</returns>
        public string ToString(bool p_b_extended, bool p_b_returnHeader)
        {
            string s_foo = "";
            s_foo += "From:\t\t" + string.Join(", ", this.FromList);
            s_foo += "\n" + "To:\t\t" + string.Join(", ", this.ToList);
            s_foo += "\n" + "CC:\t\t" + string.Join(", ", this.CCList);
            if (p_b_extended) { s_foo += "\n" + "BCC:\t\t" + string.Join(", ", this.BCCList); }
            s_foo += "\n" + "Subject:\t" + this.Subject;
            s_foo += "\n" + "Sent:\t\t" + this.Send;
            if (p_b_extended) { s_foo += "\n" + "Received:\t" + this.Received; }
            if (p_b_extended) { s_foo += "\n" + "Content-Type:\t" + this.ContentType; }
            if (p_b_extended) { s_foo += "\n" + "Expunged:\t" + this.Expunged; }
            if (p_b_extended) { s_foo += "\n" + "Description:\t" + this.Description; }
            if (p_b_extended) { s_foo += "\n" + "Disposition:\t" + this.Disposition; }
            if (p_b_extended) { s_foo += "\n" + "Size:\t\t" + this.Size; }
            s_foo += "\n" + "Flags:\t\t" + string.Join(", ", this.Flags);
            if (p_b_extended) { s_foo += "\n" + "Message-ID:\t" + this.MessageId; }

            /* return header information lines if available */
            if ((p_b_returnHeader) && (this.Header.Count > 0))
            {
                s_foo += "\n" + "Header:";

                foreach (string s_foo2 in this.Header)
                {
                    s_foo += "\n" + "\t\t\t" + s_foo2;
                }
            }

            /* if we return additional information, return plain text message as well if html message is available */
            if ((p_b_extended) && (!ForestNET.Lib.Helper.IsStringEmpty(this.Html)))
            {
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + "+++++ Message +++++";
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + this.Text;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Html))
            { /* return html message if available */
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + "++ HTML Message  ++";
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + this.Html;
            }
            else
            { /* otherwise only return plain text message */
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + "+++++ Message +++++";
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + this.Text;
            }

            /* if attachments are available, return attachment file names only */
            if ((this.Attachments != null) && (this.Attachments.Count > 0))
            {
                s_foo += "\n";
                s_foo += "\n" + "+++++++++++++++++++";
                s_foo += "\n" + "+++ Attachments +++";
                s_foo += "\n" + "+++++++++++++++++++";

                s_foo += "\n\n" + "Message-ID:\t" + this.MessageId + "\n";

                int i = 1;

                foreach (string s_attachment in this.Attachments)
                {
                    string s_filename = s_attachment.Split(Message.FIlENAMESEPARATOR)[0];
                    string s_contentType = s_attachment.Split(Message.FIlENAMESEPARATOR)[1];
                    s_foo += "\n#" + i++ + "\t" + s_filename + "(" + s_contentType + ")";
                }
            }

            return s_foo;
        }

        /// <summary>
        /// Store an attachment to local system, overwrite existing file
        /// </summary>
        /// <param name="p_i_number">number of attachment within mail message</param>
        /// <param name="p_s_path">path to local system with file name</param>
        /// <exception cref="ArgumentException">invalid number parameter</exception>
        /// <exception cref="System.IO.IOException">invalid path to local system, does not exist, cannot delete existing file or issue writing file content to local system</exception>
        /// <exception cref="ArgumentNullException">mail message has no attachments</exception>
        public void SaveAttachment(int p_i_number, string p_s_path)
        {
            this.SaveAttachment(p_i_number, p_s_path, true);
        }

        /// <summary>
        /// Store an attachment to local system
        /// </summary>
        /// <param name="p_i_number">number of attachment within mail message</param>
        /// <param name="p_s_path">path to local system with file name</param>
        /// <param name="p_b_overwrite">true - overwrite existing file, false - do not change anything on local system</param>
        /// <exception cref="ArgumentException">invalid number parameter</exception>
        /// <exception cref="System.IO.IOException">invalid path to local system, does not exist, cannot delete existing file or issue writing file content to local system</exception>
        /// <exception cref="ArgumentNullException">mail message has no attachments</exception>
        public void SaveAttachment(int p_i_number, string p_s_path, bool p_b_overwrite)
        {
            /* check number parameter */
            if (p_i_number < 1)
            {
                throw new ArgumentException("Number parameter must be at least '1'");
            }

            if ((this.AttachmentsContent != null) && (this.AttachmentsContent.Count > 0))
            {
                /* check number parameter range */
                if (p_i_number > this.Attachments.Count)
                {
                    throw new ArgumentException("Number parameter out of range, only '" + this.Attachments.Count + "' attachments available");
                }

                /* check if path to local system really exists */
                if (ForestNET.Lib.IO.File.IsDirectory(p_s_path))
                {
                    if (!p_s_path.EndsWith(ForestNET.Lib.IO.File.DIR))
                    {
                        p_s_path += ForestNET.Lib.IO.File.DIR;
                    }

                    /* add attachment file name to path */
                    p_s_path += this.Attachments[p_i_number - 1].Split(Message.FIlENAMESEPARATOR)[0];
                }

                /* check if file already exists */
                bool b_exists = ForestNET.Lib.IO.File.Exists(p_s_path);

                if ((b_exists) && (p_b_overwrite))
                { /* delete file on local system */
                    ForestNET.Lib.IO.File.DeleteFile(p_s_path);
                    b_exists = false;
                }

                /* write file content to local system */
                if (!b_exists)
                {
                    try
                    {
                        System.IO.File.WriteAllBytes(p_s_path, this.AttachmentsContent[p_i_number - 1]);
                    }
                    catch (Exception o_exc)
                    {
                        throw new System.IO.IOException("Could not write bytes to file '" + p_s_path + "'; " + o_exc.Message);
                    }
                }
                else
                {
                    throw new System.IO.IOException("File '" + p_s_path + "' already exists, and we do not overwrite.");
                }
            }
            else
            {
                throw new InvalidOperationException("E-Mail-Message has no attachments");
            }
        }

        /// <summary>
        /// Store all attachment to local system, overwrite existing files with the same name
        /// </summary>
        /// <param name="p_s_path">path to local system with file name</param>
        /// <exception cref="ArgumentException">invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">cannot delete existing file or issue writing file content to local system</exception>
        /// <exception cref="ArgumentNullException">mail message has no attachments</exception>
        public void SaveAllAttachments(string p_s_path)
        {
            this.SaveAllAttachments(p_s_path, true);
        }

        /// <summary>
        /// Store all attachment to local system
        /// </summary>
        /// <param name="p_s_path">path to local system with file name</param>
        /// <param name="p_b_overwrite">true - overwrite existing file, false - do not change anything on local system</param>
        /// <exception cref="ArgumentException">invalid path to local system</exception>
        /// <exception cref="System.IO.IOException">cannot delete existing file or issue writing file content to local system</exception>
        /// <exception cref="ArgumentNullException">mail message has no attachments</exception>
        public void SaveAllAttachments(string p_s_path, bool p_b_overwrite)
        {
            /* check if path to local system really exists */
            if (!ForestNET.Lib.IO.File.IsDirectory(p_s_path))
            {
                throw new ArgumentException("Path[" + p_s_path + "] is not a valid path");
            }

            if ((this.AttachmentsContent != null) && (this.AttachmentsContent.Count > 0))
            {
                for (int i = 0; i < this.Attachments.Count; i++)
                {
                    this.SaveAttachment(i + 1, p_s_path, p_b_overwrite);
                }
            }
        }

        /// <summary>
        /// Check if mail message has active flag.
        /// ANSWERED, DELETED, DRAFT, FLAGGED, RECENT, SEEN or USER.
        /// </summary>
        /// <param name="p_s_flag">flag string value</param>
        /// <returns>true - flag is set, false - flag is not set</returns>
        /// <exception cref="Exception">flag parameter is null or empty or flag parameter value does not correspond to specified values</exception>
        public bool HasFlag(string p_s_flag)
        {
            /* check if parameter is null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_flag))
            {
                throw new ArgumentException("Empty parameter for checking flag of E-Mail-Message.");
            }
            else
            {
                /* change all letters to upper case */
                p_s_flag = p_s_flag.ToUpper();

                /* check if parameter value corresponds to specified values  */
                if (!Message.FLAGS.Contains(p_s_flag))
                {
                    throw new ArgumentException("Flag[" + p_s_flag + "] is invalid.");
                }

                /* check if parameter value is in mail message flag list */
                if (this.a_flags.Contains(p_s_flag))
                {
                    return true;
                }
            }

            /* parameter value is not in mail message flag list */
            return false;
        }
    }
}