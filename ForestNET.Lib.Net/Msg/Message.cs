namespace ForestNET.Lib.Net.Msg
{
    /// <summary>
    /// Encapsulation of a network message where all information will be serialized into bytes.
    /// Message box id is encoded into 2 bytes [0 .. 65535].
    /// Amount of messages holding related data encoded into 2 bytes [0 .. 65535].
    /// Message number is encoded into 2 bytes [0 .. 65535].
    /// Message type length (amount of characters which describing the type of the message) is encoded into 1 byte [0 .. 255].
    /// Message type is encoded into 255 bytes [UTF-8 characters].
	/// 
    /// Following overall message lengths are valid: 1472, 1484, 1500, 8160, 8176, 8192.
    /// </summary>
    public class Message
    {

        /* Constants */
        private const int MSGBOXIDLEN = 2;
        private const int MSGAMOUNTLEN = 2;
        private const int MSGNRLEN = 2;
        private const int MSGTYPELEN = 1;
        private const int MSGTYPE = 255;

        private const int MSGLEN_1472 = 1472;
        private const int MSGLEN_1484 = 1484;
        private const int MSGLEN_1500 = 1500;
        private const int MSGLEN_8164 = 8164;
        private const int MSGLEN_8176 = 8176;
        private const int MSGLEN_8192 = 8192;
        public static readonly int[] MSGLENGTHS = { MSGLEN_1472, MSGLEN_1484, MSGLEN_1500, MSGLEN_8164, MSGLEN_8176, MSGLEN_8192 };
        public static readonly string MSGLENGTHS_STR = MSGLEN_1472 + ", " + MSGLEN_1484 + ", " + MSGLEN_1500 + ", " + MSGLEN_8164 + ", " + MSGLEN_8176 + ", " + MSGLEN_8192;

        /* Fields */

        private int i_messageLength;
        private int i_dataBlockLength;
        private readonly byte[] a_messageBoxId;
        private readonly byte[] a_messageAmount;
        private readonly byte[] a_messageNumber;
        private readonly byte[] a_typeLength;
        private readonly byte[] a_type;
        private readonly byte[] a_dataLength;
        private readonly byte[] a_data;

        /* Properties */

        public int MessageLength
        {
            get
            {
                return this.i_messageLength;
            }
            private set
            {
                this.i_messageLength = value;
            }
        }
        public int DataBlockLength
        {
            get
            {
                return this.i_dataBlockLength;
            }
            private set
            {
                this.i_dataBlockLength = value;
            }
        }
        public int MessageBoxId
        {
            get
            {
                return ForestNET.Lib.Helper.ByteArrayToInt(this.a_messageBoxId);
            }
            set
            {
                /* check message box id min value */
                if (value <= 0)
                {
                    throw new ArgumentException("Message box id must be a positive integer");
                }

                /* check message box id max value, 2^x = 1 << x -> (1 byte) -> 255 = 2 ^ [8 * (1)] - 1 | (2 byte) -> 65535 = 2 ^ [8 * (2)] - 1 */
                if (value > ((1 << (8 * MSGBOXIDLEN)) - 1))
                {
                    throw new ArgumentException("Invalid message box id[" + value + "]. Must be a positive integer lower than[" + ((1 << (8 * MSGBOXIDLEN)) - 1) + "]");
                }

                /* cast message box id int to bytes */
                byte[] a_castedMessageBoxId = ForestNET.Lib.Helper.IntToByteArray(value) ?? [];

                /* copy bytes to byte array for message box id */
                for (int i = this.a_messageBoxId.Length; i > 0; i--)
                {
                    /* reached last byte? -> break */
                    if (((i - 1) + (a_castedMessageBoxId.Length - this.a_messageBoxId.Length)) < 0)
                    {
                        break;
                    }

                    /* copy byte to array */
                    this.a_messageBoxId[i - 1] = a_castedMessageBoxId[(i - 1) + (a_castedMessageBoxId.Length - this.a_messageBoxId.Length)];
                }
            }
        }
        public int MessageAmount
        {
            get
            {
                return ForestNET.Lib.Helper.ByteArrayToInt(this.a_messageAmount);
            }
            set
            {
                /* check message amount min value */
                if (value <= 0)
                {
                    throw new ArgumentException("Message amount must be a positive integer");
                }

                /* check message amount max value, 2^x = 1 << x -> (1 byte) -> 255 = 2 ^ [8 * (1)] - 1 | (2 byte) -> 65535 = 2 ^ [8 * (2)] - 1 */
                if (value > ((1 << (8 * MSGAMOUNTLEN)) - 1))
                {
                    throw new ArgumentException("Invalid message amount[" + value + "]. Must be a positive integer lower than[" + ((1 << (8 * MSGAMOUNTLEN)) - 1) + "]");
                }

                /* cast message amount int to bytes */
                byte[] a_castedMessageAmount = ForestNET.Lib.Helper.IntToByteArray(value) ?? [];

                /* copy bytes to byte array for message amount */
                for (int i = this.a_messageAmount.Length; i > 0; i--)
                {
                    /* reached last byte? -> break */
                    if (((i - 1) + (a_castedMessageAmount.Length - this.a_messageAmount.Length)) < 0)
                    {
                        break;
                    }

                    /* copy byte to array */
                    this.a_messageAmount[i - 1] = a_castedMessageAmount[(i - 1) + (a_castedMessageAmount.Length - this.a_messageAmount.Length)];
                }
            }
        }
        public int MessageNumber
        {
            get
            {
                return ForestNET.Lib.Helper.ByteArrayToInt(this.a_messageNumber);
            }
            set
            {
                /* check message number min value */
                if (value <= 0)
                {
                    throw new ArgumentException("Message number must be a positive integer");
                }

                /* check message number max value, 2^x = 1 << x -> (1 byte) -> 255 = 2 ^ [8 * (1)] - 1 | (2 byte) -> 65535 = 2 ^ [8 * (2)] - 1 */
                if (value > ((1 << (8 * MSGNRLEN)) - 1))
                {
                    throw new ArgumentException("Invalid message number[" + value + "]. Must be a positive integer lower than[" + ((1 << (8 * MSGNRLEN)) - 1) + "]");
                }

                /* cast message number int to bytes */
                byte[] a_castedMessageNumber = ForestNET.Lib.Helper.IntToByteArray(value) ?? [];

                /* copy bytes to byte array for message number */
                for (int i = this.a_messageNumber.Length; i > 0; i--)
                {
                    /* reached last byte? -> break */
                    if (((i - 1) + (a_castedMessageNumber.Length - this.a_messageNumber.Length)) < 0)
                    {
                        break;
                    }

                    /* copy byte to array */
                    this.a_messageNumber[i - 1] = a_castedMessageNumber[(i - 1) + (a_castedMessageNumber.Length - this.a_messageNumber.Length)];
                }
            }
        }
        public string Type
        {
            get
            {
                /* create string bytes array */
                byte[] a_stringBytes = new byte[this.a_typeLength[0]];

                /* iterate each character byte, length stored in type length array (MSGTYPELEN -> 1 byte) */
                for (int i = 0; i < this.a_typeLength[0]; i++)
                {
                    /* copy character byte to string byte array */
                    a_stringBytes[i] = this.a_type[i];
                }

                /* return string with string bytes array */
                return System.Text.Encoding.UTF8.GetString(a_stringBytes);
            }
            set
            {
                /* convert parameter string to byte array */
                byte[] a_stringType = System.Text.Encoding.UTF8.GetBytes(value);

                /* check if byte array length does not exceed max value */
                if (a_stringType.Length > Message.MSGTYPE)
                {
                    /* try to shorten the type in this special case, e.g. List<DateTime?> */
                    if (value.Contains("]],"))
                    {
                        value = value.Substring(0, value.LastIndexOf("]],") + 2);
                        value = System.Text.RegularExpressions.Regex.Replace(value, ", ", ",");

                        /* convert parameter string to byte array */
                        a_stringType = System.Text.Encoding.UTF8.GetBytes(value);

                        /* check if byte array length does not exceed max value */
                        if (a_stringType.Length > Message.MSGTYPE)
                        {
                            throw new ArgumentException("Type has max length[" + Message.MSGTYPE + "], but parameter is byte[" + value.Length + "] long -> '" + value + "'");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Type has max length[" + Message.MSGTYPE + "], but parameter is byte[" + value.Length + "] long -> '" + value + "'");
                    }
                }

                /* convert length of byte array to a byte value itself; because MSGTYPE -> 255 we always get only one byte as return value */
                this.a_typeLength[0] = (byte)a_stringType.Length;

                /* copy byte array to type array */
                for (int i = 0; i < a_stringType.Length; i++)
                {
                    this.a_type[i] = a_stringType[i];
                }
            }
        }
        public int DataLength
        {
            get
            {
                return ForestNET.Lib.Helper.ByteArrayToInt(this.a_dataLength);
            }
            set
            {
                /* set data length byte array to zero values; it's length is determined in Message constructor */
                for (int i = 0; i < this.a_dataLength.Length; i++)
                {
                    this.a_dataLength[i] = 0;
                }

                /* cast data length int to bytes */
                byte[] a_castedDataLength = ForestNET.Lib.Helper.IntToByteArray(value) ?? [];

                /* copy bytes to byte array for data length */
                for (int i = this.a_dataLength.Length; i > 0; i--)
                {
                    /* reached last byte? -> break */
                    if (((i - 1) + (a_castedDataLength.Length - this.a_dataLength.Length)) < 0)
                    {
                        break;
                    }

                    /* copy byte to array */
                    this.a_dataLength[i - 1] = a_castedDataLength[(i - 1) + (a_castedDataLength.Length - this.a_dataLength.Length)];
                }
            }
        }
        public byte[] Data
        {
            get
            {
                return this.a_data;
            }
            set
            {
                /* check if amount of data bytes does not exceed determined data block length from constructor */
                if (value.Length > this.i_dataBlockLength)
                {
                    throw new ArgumentException("Data is byte[" + this.i_dataBlockLength + "], but parameter is byte[" + value.Length + "]");
                }

                /* set data length byte array with data length integer value */
                this.DataLength = value.Length;

                /* copy parameter data to data bytes array */
                for (int i = 0; i < value.Length; i++)
                {
                    this.a_data[i] = value[i];
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Create network message length with a specific length
        /// </summary>
        /// <param name="p_i_length">overall length of network message, valid values are within Message.MSGLENGTHS</param>
        /// <exception cref="ArgumentException">exceeded max message length</exception>
        public Message(int p_i_length)
        {
            /* check message length parameter */
            if (p_i_length > MSGLEN_8192)
            {
                throw new ArgumentException("Invalid length[" + p_i_length + "] for message. Max message length is [" + MSGLEN_8192 + "]");
            }

            /* auto assign message length */
            if (p_i_length <= MSGLEN_1472)
            {
                p_i_length = MSGLEN_1472;
            }
            else if ((p_i_length > MSGLEN_1472) && (p_i_length <= MSGLEN_1484))
            {
                p_i_length = MSGLEN_1484;
            }
            else if ((p_i_length > MSGLEN_1484) && (p_i_length <= MSGLEN_1500))
            {
                p_i_length = MSGLEN_1500;
            }
            else if ((p_i_length > MSGLEN_1500) && (p_i_length <= MSGLEN_8164))
            {
                p_i_length = MSGLEN_8164;
            }
            else if ((p_i_length > MSGLEN_8164) && (p_i_length <= MSGLEN_8176))
            {
                p_i_length = MSGLEN_8176;
            }
            else
            {
                p_i_length = MSGLEN_8192;
            }

            /* initiate byte arrays */
            this.i_messageLength = p_i_length;
            this.a_messageBoxId = new byte[Message.MSGBOXIDLEN];
            this.a_messageAmount = new byte[Message.MSGAMOUNTLEN];
            this.a_messageNumber = new byte[Message.MSGNRLEN];
            this.a_typeLength = new byte[Message.MSGTYPELEN];
            this.a_type = new byte[Message.MSGTYPE];

            /* get data block length by taking message length and subtracting all byte array lengths */
            this.i_dataBlockLength = this.i_messageLength - this.a_messageBoxId.Length - this.a_messageAmount.Length - this.a_messageNumber.Length - this.a_typeLength.Length - this.a_type.Length;

            /* determine how many bytes we need to store data length --- length for data length = log10(data block length) / log10(2) / 8 */
            int i_lengthByteData = (int)Math.Ceiling(Math.Log10(this.i_dataBlockLength) / Math.Log10(2));
            i_lengthByteData = (int)Math.Ceiling((double)i_lengthByteData / 8.0d);

            /* initiate data length byte array */
            this.a_dataLength = new byte[i_lengthByteData];

            /* data block length decreased with bytes we need to store data length */
            this.i_dataBlockLength -= i_lengthByteData;

            /* initiate data byte array */
            this.a_data = new byte[this.i_dataBlockLength];
        }

        /// <summary>
        /// Get data block length by giving overall message length
        /// </summary>
        /// <param name="p_i_length">overall length of network message, valid values are within Message.MSGLENGTHS</param>
        /// <returns>int</returns>
        /// <exception cref="ArgumentException">invalid length for network message</exception>
        public static int CalculateDataBlockLength(int p_i_length)
        {
            int i_return;

            /* check length parameter */
            if (!Message.MSGLENGTHS.Contains(p_i_length))
            {
                throw new ArgumentException("Invalid length[" + p_i_length + "] for message. Valid values are [" + Message.MSGLENGTHS_STR + "]");
            }

            /* get data block length by taking message length and subtracting all byte array lengths */
            i_return = p_i_length - Message.MSGBOXIDLEN - Message.MSGAMOUNTLEN - Message.MSGNRLEN - Message.MSGTYPELEN - Message.MSGTYPE;

            /* determine how many bytes we need to store data length --- length for data length = log10(data block length) / log10(2) / 8 */
            int i_lengthByteData = (int)Math.Ceiling(Math.Log10(i_return) / Math.Log10(2));
            i_lengthByteData = (int)Math.Ceiling((double)i_lengthByteData / 8.0d);

            /* data block length decreased with bytes we need to store data length */
            i_return -= i_lengthByteData;

            /* return data block length */
            return i_return;
        }

        /// <summary>
        /// Get complete message object with all information as byte array
        /// </summary>
        /// <returns>byte[] array</returns>
        public byte[] GetByteArrayFromMessage()
        {
            /* return value */
            byte[] a_message = new byte[this.i_messageLength];

            /* index values */
            int i, j, k, l, m, n, o;

            /* handle message box id */
            for (i = 0; i < this.a_messageBoxId.Length; i++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageBoxId{" + (i + 1) + " of " + this.a_messageBoxId.Length + "}: [" + (i) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_messageBoxId[i] }, false));
                a_message[i] = this.a_messageBoxId[i];
            }

            /* handle message amount */
            for (j = 0; j < this.a_messageAmount.Length; j++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageAmount{" + (j + 1) + " of " + this.a_messageAmount.Length + "}: [" + (i + j) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_messageAmount[j] }, false));
                a_message[i + j] = this.a_messageAmount[j];
            }

            /* handle message number */
            for (k = 0; k < this.a_messageNumber.Length; k++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageNumber{" + (k + 1) + " of " + this.a_messageNumber.Length + "}: [" + (i + j + k) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_messageNumber[k] }, false));
                a_message[i + j + k] = this.a_messageNumber[k];
            }

            /* handle message type length */
            for (l = 0; l < this.a_typeLength.Length; l++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageTypeLength{" + (l + 1) + " of " + this.a_typeLength.Length + "}: [" + (i + j + k + l) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_typeLength[l] }, false));
                a_message[i + j + k + l] = this.a_typeLength[l];
            }

            /* handle message type */
            for (m = 0; m < this.a_type.Length; m++)
            {
                if (m < this.a_typeLength[0])
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageType{" + (m + 1) + " of " + this.a_type.Length + "}: [" + (i + j + k + l + m) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_type[m] }, false));
                }

                a_message[i + j + k + l + m] = this.a_type[m];
            }

            /* handle message data length */
            for (n = 0; n < this.a_dataLength.Length; n++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageDataLength{" + (n + 1) + " of " + this.a_dataLength.Length + "}: [" + (i + j + k + l + m + n) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_dataLength[n] }, false));
                a_message[i + j + k + l + m + n] = this.a_dataLength[n];
            }

            /* handle message data */
            for (o = 0; o < this.a_data.Length; o++)
            {
                /* reached last byte? -> break */
                if ((i + j + k + l + m + n + o) >= this.i_messageLength)
                {
                    break;
                }

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageData{" + (o + 1) + " of " + this.a_data.Length + "}: [" + (i + j + k + l + m + n + o) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { this.a_data[o] }, false));
                a_message[i + j + k + l + m + n + o] = this.a_data[o];
            }

            /* return byte array */
            return a_message;
        }

        /// <summary>
        /// Set complete message object with all information from a byte array
        /// </summary>
        /// <param name="p_a_message">byte array containing all message information</param>
        /// <exception cref="ArgumentException">parameter byte array has not the minimal message length or is bigger than the maximal message length</exception>
        public void SetMessageFromByteArray(byte[] p_a_message)
        {
            int i_minimalMessageLength = (this.a_messageBoxId.Length + this.a_messageAmount.Length + this.a_messageNumber.Length + this.a_typeLength.Length + this.a_type.Length);

            /* check for minimal message length */
            if (p_a_message.Length < i_minimalMessageLength)
            {
                throw new ArgumentException("Amount of message bytes[" + p_a_message.Length + "] must be greater than minimal message length[" + i_minimalMessageLength + "]");
            }

            /* check for maximal message length */
            if (p_a_message.Length > this.i_messageLength)
            {
                throw new ArgumentException("Amount of message bytes[" + p_a_message.Length + "] must be lower than allowed message length[" + this.i_messageLength + "]");
            }

            /* index values */
            int i, j, k, l, m, n, o;

            /* read message box id from byte array */
            for (i = 0; i < this.a_messageBoxId.Length; i++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageBoxId{" + (i + 1) + " of " + this.a_messageBoxId.Length + "}: [" + (i) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i] }, false));
                this.a_messageBoxId[i] = p_a_message[i];
            }

            /* read message amount from byte array */
            for (j = 0; j < this.a_messageAmount.Length; j++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageAmount{" + (j + 1) + " of " + this.a_messageAmount.Length + "}: [" + (i + j) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j] }, false));
                this.a_messageAmount[j] = p_a_message[i + j];
            }

            /* read message number from byte array */
            for (k = 0; k < this.a_messageNumber.Length; k++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageNumber{" + (k + 1) + " of " + this.a_messageNumber.Length + "}: [" + (i + j + k) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j + k] }, false));
                this.a_messageNumber[k] = p_a_message[i + j + k];
            }

            /* read message type length from byte array */
            for (l = 0; l < this.a_typeLength.Length; l++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageTypeLength{" + (l + 1) + " of " + this.a_typeLength.Length + "}: [" + (i + j + k + l) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j + k + l] }, false));
                this.a_typeLength[l] = p_a_message[i + j + k + l];
            }

            /* read message type from byte array */
            for (m = 0; m < this.a_type.Length; m++)
            {
                if (m < this.a_typeLength[0])
                {
                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageType{" + (m + 1) + " of " + this.a_type.Length + "}: [" + (i + j + k + l + m) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j + k + l + m] }, false));
                }

                this.a_type[m] = p_a_message[i + j + k + l + m];
            }

            /* read message data length from byte array */
            for (n = 0; n < this.a_dataLength.Length; n++)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageDataLength{" + (n + 1) + " of " + this.a_dataLength.Length + "}: [" + (i + j + k + l + m + n) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j + k + l + m + n] }, false));
                this.a_dataLength[n] = p_a_message[i + j + k + l + m + n];
            }

            /* read message data from byte array */
            for (o = 0; o < this.a_data.Length; o++)
            {
                /* reached last byte? -> break */
                if ((i + j + k + l + m + n + o) >= this.i_messageLength)
                {
                    break;
                }

                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("MessageData{" + (o + 1) + " of " + this.a_data.Length + "}: [" + (i + j + k + l + m + n + o) + "] " + ForestNET.Lib.Helper.PrintByteArray(new byte[] { p_a_message[i + j + k + l + m + n + o] }, false));
                this.a_data[o] = p_a_message[i + j + k + l + m + n + o];
            }
        }
    }
}
