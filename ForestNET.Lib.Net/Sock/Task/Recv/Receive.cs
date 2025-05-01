namespace ForestNET.Lib.Net.Sock.Task.Recv
{
    /// <summary>
    /// Generic task class to receive network traffic over a socket instance. Several methods supporting UDP and TCP, receiving data in combination with a message box or multiple message boxes.
    /// </summary>
    public class Receive : ForestNET.Lib.Net.Sock.Task.Task
    {
        /// <summary>
        /// Parameterless constructor for TCP RunServer method
        /// </summary>
        public Receive() : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_SERVER)
        {

        }

        /// <summary>
        /// Creating receiving socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Receive(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds) :
            base(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds)
        {

        }

        /// <summary>
        /// Creating receiving socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Receive(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity) :
            base(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, p_e_communicationSecurity)
        {

        }

        /// <summary>
        /// Creating receiving socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <param name="p_b_useMarshalling">true - use marshalling methods to transport data over network</param>
        /// <param name="p_i_marshallingDataLengthInBytes">set data length in bytes for marshalling, must be between [1..4]</param>
        /// <param name="p_b_marshallingUseProperties">true - access object parameter fields via properties</param>
        /// <param name="p_s_marshallingOverrideMessageType">override message type with this string and do not get it automatically from object, thus the type can be set generally from other systems with other programming languages</param>
        /// <param name="p_b_marshallingSystemUsesLittleEndian">(NOT IMPLEMENTED) true - current execution system uses little endian, false - current execution system uses big endian</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Receive(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian) :
            base(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, p_e_communicationSecurity, null, p_b_useMarshalling, p_i_marshallingDataLengthInBytes, p_b_marshallingUseProperties, p_s_marshallingOverrideMessageType, p_b_marshallingSystemUsesLittleEndian)
        {

        }

        /// <summary>
        /// method to clone this socket task with another socket task instance
        /// </summary>
        /// <param name="p_o_sourceTask">another socket task instance as source for all it's parameters and settings</param>
        public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
        {
            this.CloneBasicProperties(p_o_sourceTask);
        }

        /// <summary>
        /// runTask method of receiving socket task which can always vary depending on the implementation. Supporting in this class UDP_RECEIVE, UDP_RECEIVE_WITH_ACK, TCP_RECEIVE and TCP_RECEIVE_WITH_ANSWER.
        /// </summary>
        /// <exception cref="Exception">any exception of implementation that could happen will be caught by abstract Task class, see details in protocol methods in ForestNET.Lib.Net.Sock.Task.Recv.Receive</exception>
        public override async System.Threading.Tasks.Task RunTask()
        {
            if ((this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE) || (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK))
            {
                /* receive UDP packets */
                await this.UDPReceive();
            }
            else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE)
            {
                /* check if we want to use tcp socket for bidirectional communication */
                if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional)
                {
                    await this.TCPBidirectional();
                }
                else
                {
                    if (this.ObjectTransmission)
                    {
                        /* receive TCP packets, but a whole object, so several messages of a message box until the object has been transferred */
                        await this.TCPReceiveObjectTransmission();
                    }
                    else
                    {
                        /* receive TCP packet */
                        await this.TCPReceive();
                    }
                }
            }
            else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_RECEIVE_WITH_ANSWER)
            {
                /* receive TCP packet and answer immediately with receiveSocketTask property */
                await this.TCPReceiveWithAnswer();
            }
        }

        /// <summary>
        /// Method for handling a received UDP datagram packet, optional with sending an acknowledge(ACK) over UDP which is not UDP standard but could be useful sometimes
        /// </summary>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        private async System.Threading.Tasks.Task UDPReceive()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* if we received no datagram bytes we have no message information as well */
            if (this.DatagramBytes == null)
            {
                return;
            }

            ForestNET.Lib.Global.ILogFiner("Received datagram packet, length = " + this.DatagramBytes.Length + " bytes");

            /* if we received no data we have no message information as well */
            if (this.DatagramBytes.Length < 1)
            {
                return;
            }

            /* communication type requires to send an acknowledge(ACK) over UDP */
            if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_RECEIVE_WITH_ACK)
            {
                /* check if we have a udp source address */
                if (this.UDPSourceAddress != null)
                {
                    /* we wait some time so the other side of communication can prepare to receive UDP ACK */
                    await System.Threading.Tasks.Task.Delay(this.UDPSendACKTimeoutMilliseconds, this.CancellationToken ?? default);

                    ForestNET.Lib.Global.ILogFiner("Received data, now sendACK(" + this.UDPSourceAddress + ")");

                    /* send UDP ACK to datagram source address and source port */
                    await this.SendUdpAck(this.UDPSourceAddress);
                }
                else /* cannot send ACK, UDP source address is null */
                {
                    ForestNET.Lib.Global.ILogWarning("cannot send ACK, UDP source address is null");
                }
            }

            /* get message length */
            int i_length = this.DatagramBytes.Length;

            /* our buffer expects fewer bytes than we received */
            if (this.BufferLength < this.DatagramBytes.Length)
            {
                ForestNET.Lib.Global.ILogFiner("Our buffer expects fewer bytes than we received: '" + this.BufferLength + "' < '" + this.DatagramBytes.Length + "'");

                /* new byte array with expected buffer length */
                byte[] a_foo = new byte[this.BufferLength];

                /* copy byte array data */
                for (int i = 0; i < a_foo.Length; i++)
                {
                    a_foo[i] = this.DatagramBytes[i];
                }

                ForestNET.Lib.Global.ILogFiner("Only read '" + a_foo.Length + "' bytes of received data");

                this.DatagramBytes = a_foo;
            }

            /* decrypt received bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                this.DatagramBytes = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(this.DatagramBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                i_length = this.DatagramBytes.Length;
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                this.DatagramBytes = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(this.DatagramBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                i_length = this.DatagramBytes.Length;
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                this.DatagramBytes = this.Cryptography?.Decrypt(this.DatagramBytes) ?? [];
                i_length = this.DatagramBytes.Length;
            }

            ForestNET.Lib.Global.ILogFiner("received data (optional already decrypted), data length = " + i_length + " bytes");

            /* create message object with message length and message data */
            ForestNET.Lib.Net.Msg.Message o_message = new(i_length);
            o_message.SetMessageFromByteArray(this.DatagramBytes);

            ForestNET.Lib.Global.ILogFine("Received message from[" + this.UDPSourceAddress + "]: message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + o_message.MessageLength + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_message.MessageBoxId + "]");

            /* debug received message if b_debugNetworkTrafficOn is set to true */
            this.DebugMessage(this.DatagramBytes, o_message);

            int i_messageBoxId = 0;

            /* determine message box id if we have a cardinality with many message boxes and one socket */
            if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket)
            {
                i_messageBoxId = o_message.MessageBoxId;

                /* check message box id min. value */
                if (i_messageBoxId < 1)
                {
                    throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] must be greater than [0].");
                }

                /* check message box id max. value */
                if (i_messageBoxId > this.MessageBoxes.Count)
                {
                    throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] is not available.");
                }

                i_messageBoxId--;
            }

            /* enqueue message object */
            while (!this.MessageBoxes[i_messageBoxId].EnqueueMessage(o_message))
            {
                ForestNET.Lib.Global.ILogWarning("Could not enqueue message object, timeout for '" + this.QueueTimeoutMilliseconds + "' milliseconds");

                /* wait queue timeout length to enqueue message object again */
                await System.Threading.Tasks.Task.Delay(this.QueueTimeoutMilliseconds, this.CancellationToken ?? default);
            }
            ;
        }

        /// <summary>
        /// Send UDP ACK to datagram source address and source port. All exceptions will be handled within this method and are not thrown to parent methods.
        /// </summary>
        /// <param name="p_o_udpSourceAddress">destination address for UDP ACK</param>
        private async System.Threading.Tasks.Task SendUdpAck(System.Net.IPEndPoint p_o_udpSourceAddress)
        {
            try
            {
                /* create UDP socket for sending ACK */
                System.Net.Sockets.Socket o_sendSocket = new(p_o_udpSourceAddress.AddressFamily, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                try
                {
                    /* set destination address to source of received UDP datagram packet */
                    await o_sendSocket.ConnectAsync(p_o_udpSourceAddress);

                    /* prepare ACK byte of abstract Task class */
                    byte[] a_bytes = new byte[] { (byte)ForestNET.Lib.Net.Sock.Task.Task.BY_ACK_BYTE };

                    /* encrypt acknowledge byte if encryption is active */
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                    {
                        a_bytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_bytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                    {
                        a_bytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_bytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_bytes = this.Cryptography?.Encrypt(a_bytes) ?? [];
                    }

                    /* set receive and send buffer size */
                    o_sendSocket.ReceiveBufferSize = a_bytes.Length;
                    o_sendSocket.SendBufferSize = a_bytes.Length;

                    /* send datagram packet with UDP socket object */
                    int i_bytesSend = await o_sendSocket.SendAsync(a_bytes, System.Net.Sockets.SocketFlags.None, this.CancellationToken ?? default);

                    ForestNET.Lib.Global.ILogFiner("sent UDP ACK o_sendSocket.SendAsync() with '" + i_bytesSend + " bytes'");
                }
                catch (System.Net.Sockets.SocketException o_exc)
                {
                    ForestNET.Lib.Global.LogException("SocketException Receive-SendUdpAck method: ", o_exc);
                }
                finally
                {
                    /* close socket */
                    o_sendSocket.Shutdown(System.Net.Sockets.SocketShutdown.Send);
                    o_sendSocket.Close();
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception Receive-SendUdpAck method: ", o_exc);
            }
        }

        /// <summary>
        /// Method for handling receiving bytes from a TCP connection, until all data is gathered for a whole object
        /// </summary>
        /// <exception cref="InvalidOperationException">amount of data is not a multiple of buffer length</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        private async System.Threading.Tasks.Task TCPReceiveObjectTransmission()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, starting");

            /* use AmountBytesProtocol to know how many bytes and in that way how many messages we are expecting */
            int i_amountBytes = await this.AmountBytesProtocol();

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, finished, length = " + i_amountBytes + " bytes");

            /* no encryption active */
            if ((this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                /* check if amount of data is not a multiple of buffer length */
                if ((i_amountBytes % this.BufferLength) != 0)
                {
                    throw new InvalidOperationException("Invalid amount of data[" + i_amountBytes + "]. Amount of data is not a multiple of buffer length[" + this.BufferLength + "].");
                }
            }

            /* receive data bytes of all expected messages for object */
            byte[]? a_receivedData = await this.ReceiveBytes(i_amountBytes);

            /* if we received no data we have no message information as well */
            if ((a_receivedData == null) || (a_receivedData.Length < 1))
            {
                return;
            }

            /* decrypt received bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_receivedData = this.Cryptography?.Decrypt(a_receivedData);
            }

            ForestNET.Lib.Global.ILogFiner("received data (optional already decrypted), length = " + (a_receivedData?.Length ?? 0) + " bytes");

            int i_decreaseForEncryption = 0;

            /* check for length correction if encryption is active */
            if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                {
                    i_decreaseForEncryption = 28;
                }
                else
                {
                    i_decreaseForEncryption = 16;
                }

                /* check if amount of decrypted data is not a multiple of buffer length */
                if (((a_receivedData?.Length ?? 0) % (this.BufferLength - i_decreaseForEncryption)) != 0)
                {
                    throw new InvalidOperationException("Invalid amount of data[" + (a_receivedData?.Length ?? 0) + "]. Amount of data is not a multiple of buffer length[" + (this.BufferLength - i_decreaseForEncryption) + "].");
                }
            }

            /* calculate how many messages we expect to receive until all object data has been gathered */
            int i_messages = (a_receivedData?.Length ?? 0) / (this.BufferLength - i_decreaseForEncryption);
            int i_messageBoxId;

            ForestNET.Lib.Global.ILogFine("Received message from[" + this.ReceivingSocket?.RemoteEndPoint + "]: messages[" + i_messages + "], length[" + (a_receivedData?.Length ?? 0) + "]");

            /* receive other expected messages until all object data has been gathered */
            for (int i = 0; i < i_messages; i++)
            {
                /* create message data array */
                byte[] a_messageData = new byte[(this.BufferLength - i_decreaseForEncryption)];

                /* copy received data into message data array */
                if (a_receivedData != null)
                {
                    for (int i_bytePointer = 0; i_bytePointer < (this.BufferLength - i_decreaseForEncryption); i_bytePointer++)
                    {
                        a_messageData[i_bytePointer] = a_receivedData[i_bytePointer + (i * (this.BufferLength - i_decreaseForEncryption))];
                    }
                }

                /* create message object with message length and message data */
                ForestNET.Lib.Net.Msg.Message o_message = new(this.BufferLength - i_decreaseForEncryption);
                o_message.SetMessageFromByteArray(a_messageData);

                ForestNET.Lib.Global.ILogFine("Received message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + o_message.MessageLength + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_message.MessageBoxId + "]");

                /* debug received message if b_debugNetworkTrafficOn is set to true */
                this.DebugMessage(a_messageData, o_message);

                i_messageBoxId = 0;

                /* determine message box id if we have a cardinality with many message boxes and one socket */
                if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.ManyMessageBoxesToOneSocket)
                {
                    i_messageBoxId = o_message.MessageBoxId;

                    /* check message box id min. value */
                    if (i_messageBoxId < 1)
                    {
                        throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] must be greater than [0].");
                    }

                    /* check message box id max. value */
                    if (i_messageBoxId > this.MessageBoxes.Count)
                    {
                        throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] is not available.");
                    }

                    i_messageBoxId--;
                }

                /* enqueue message object */
                while (!this.MessageBoxes[i_messageBoxId].EnqueueMessage(o_message))
                {
                    ForestNET.Lib.Global.ILogWarning("Could not enqueue message, timeout for '" + this.QueueTimeoutMilliseconds + "' milliseconds");

                    /* wait queue timeout length to enqueue message object again */
                    await System.Threading.Tasks.Task.Delay(this.QueueTimeoutMilliseconds, this.CancellationToken ?? default);
                }
                ;
            }

            /* with asymmetric encryption we have to close our ssl socket ourselves */
            if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.ASYMMETRIC) && (this.ReceivingSocket?.Connected ?? false))
            {
                this.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                this.ReceivingSocket?.Close();
            }
        }

        /// <summary>
        /// Method for receiving an object from a TCP connection, until all data is gathered and an answer can be send with separate receive socket task
        /// </summary>
        /// <exception cref="InvalidOperationException">amount of data is not a multiple of buffer length</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details, but also any Exception within receive socket task or casting request object and/or answer object</exception>
        private async System.Threading.Tasks.Task TCPReceiveWithAnswer()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* check if receive socket task is initiated */
            if (this.ReceiveSocketTask == null)
            {
                throw new ArgumentNullException("Receive socket task is null");
            }

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, starting");

            /* use AmountBytesProtocol to know how many bytes and in that way how many messages we are expecting */
            int i_amountBytes = await this.AmountBytesProtocol();

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, finished, length = " + i_amountBytes + " bytes");

            /* no encryption active */
            if ((this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) && (this.CommunicationSecurity != ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                /* check if amount of data is not a multiple of buffer length */
                if ((i_amountBytes % this.BufferLength) != 0)
                {
                    throw new InvalidOperationException("Invalid amount of data[" + i_amountBytes + "]. Amount of data is not a multiple of buffer length[" + this.BufferLength + "].");
                }
            }

            /* receive data bytes of all expected messages for request object */
            byte[]? a_receivedData = await this.ReceiveBytes(i_amountBytes);

            /* if we received no data we have no message information as well */
            if ((a_receivedData == null) || (a_receivedData.Length < 1))
            {
                return;
            }

            /* decrypt received bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_receivedData = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_receivedData, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_receivedData = this.Cryptography?.Decrypt(a_receivedData);
            }

            ForestNET.Lib.Global.ILogFiner("received data (optional already decrypted), length = " + (a_receivedData?.Length ?? 0) + " bytes");

            int i_decreaseForEncryption = 0;

            /* check for length correction if encryption is active  */
            if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                {
                    i_decreaseForEncryption = 28;
                }
                else
                {
                    i_decreaseForEncryption = 16;
                }

                /* check if amount of decrypted data is not a multiple of buffer length */
                if (((a_receivedData?.Length ?? 0) % (this.BufferLength - i_decreaseForEncryption)) != 0)
                {
                    throw new InvalidOperationException("Invalid amount of data[" + (a_receivedData?.Length ?? 0) + "]. Amount of data is not a multiple of buffer length[" + (this.BufferLength - i_decreaseForEncryption) + "].");
                }
            }

            /* help variables to create request object out of received data */
            Object? o_object;
            string? s_objectType = null;
            int i_objectDataLength = 0;
            List<byte> a_bytes = [];

            /* calculate how many messages we expect to receive until all object data has been gathered */
            int i_messages = (a_receivedData?.Length ?? 0) / (this.BufferLength - i_decreaseForEncryption);

            ForestNET.Lib.Global.ILogFine("Received message from[" + this.ReceivingSocket?.RemoteEndPoint + "]: messages[" + i_messages + "], length[" + (a_receivedData?.Length ?? 0) + "]");

            /* receive other expected messages until all object data has been gathered */
            for (int i = 0; i < i_messages; i++)
            {
                /* create message data array */
                byte[] a_messageData = new byte[(this.BufferLength - i_decreaseForEncryption)];

                /* copy received data into message data array */
                if (a_receivedData != null)
                {
                    for (int i_bytePointer = 0; i_bytePointer < (this.BufferLength - i_decreaseForEncryption); i_bytePointer++)
                    {
                        a_messageData[i_bytePointer] = a_receivedData[i_bytePointer + (i * (this.BufferLength - i_decreaseForEncryption))];
                    }
                }

                /* create message object with message length and message data */
                ForestNET.Lib.Net.Msg.Message o_message = new(this.BufferLength - i_decreaseForEncryption);
                o_message.SetMessageFromByteArray(a_messageData);
                /* get object type and increase object data length */
                s_objectType = o_message.Type;
                i_objectDataLength += o_message.DataLength;

                ForestNET.Lib.Global.ILogFiner("Received message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + o_message.MessageLength + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_message.MessageBoxId + "]");

                /* debug received message if b_debugNetworkTrafficOn is set to true */
                this.DebugMessage(a_messageData, o_message);

                /* gather bytes for request object */
                for (int j = 0; j < o_message.DataLength; j++)
                {
                    a_bytes.Add(o_message.Data[j]);
                }
            }

            /* create byte array for request object */
            byte[] a_objectBytes = new byte[i_objectDataLength];
            int k = 0;

            /* copy received bytes to byte array for request object */
            foreach (byte by_byte in a_bytes)
            {
                a_objectBytes[k++] = by_byte;
            }

            /* check if we use marshalling for network communication */
            if (this.UseMarshalling)
            {
                /* get unmarshalled object from messages */
                o_object = ForestNET.Lib.Net.Msg.MessageBox.UnmarshallObjectFromMessage(s_objectType ?? "missing message instance for type", a_objectBytes, this.MarshallingUseProperties, this.MarshallingSystemUsesLittleEndian);
            }
            else
            { /* no marshalling for network communication */
                /* read byte array to object to complete merging object data */
                try
                {
                    /* get message type */
                    string s_typeFoo = s_objectType ?? "missing message instance for type";
                    /* get origin type of enqueued data */
                    System.Type o_targetType = System.Type.GetType(s_typeFoo) ?? throw new NullReferenceException("Could not retrieve object type with '" + s_typeFoo + "'");
                    /* deserialize data */
                    o_object = System.Text.Json.JsonSerializer.Deserialize(a_objectBytes, o_targetType);
                }
                catch (Exception o_exc)
                {
                    ForestNET.Lib.Global.ILogFinest("could not read merged object data: " + o_exc);

                    o_object = null;
                }
            }

            /* check if cast was successful */
            if (o_object == null)
            {
                throw new ArgumentNullException("Could not deserialize and cast object from received bytes");
            }

            ForestNET.Lib.Global.ILogFiner("pass request object to receive socket task (user/developer implementation)");

            /* pass request object to receive socket task (user/developer implementation) */
            this.ReceiveSocketTask.RequestObject = o_object;

            ForestNET.Lib.Global.ILogFine("execute receive socket task (user/developer implementation)");

            /* execute receive socket task (user/developer implementation) */
            await this.ReceiveSocketTask.RunTask();

            ForestNET.Lib.Global.ILogFiner("retrieve answer object of receive socket task (user/developer implementation)");

            /* retrieve answer object of receive socket task (user/developer implementation) */
            o_object = this.ReceiveSocketTask.AnswerObject;

            /* check if answer object was retrieved successful */
            if (o_object == null)
            {
                throw new ArgumentNullException("Cannot send answer, answer object is null");
            }

            /* byte array for sending answer object to other communication side */
            byte[]? a_answerBytes;
            byte[]? a_answerObjectBytes;
            string s_type;

            ForestNET.Lib.Global.ILogFine("serialize answer object from receive socket task (user/developer implementation)");

            /* check if we use marshalling for network communication */
            if (this.UseMarshalling)
            {
                try
                {
                    /* call marshall object method */
                    a_answerObjectBytes = ForestNET.Lib.Net.Msg.Marshall.MarshallObject(o_object, this.MarshallingDataLengthInBytes, this.MarshallingUseProperties, this.MarshallingSystemUsesLittleEndian);

                    /* get universal primitive type name if available */
                    if (o_object == null)
                    {
                        s_type = "null";
                    }
                    else if (o_object.GetType() == typeof(bool))
                    {
                        s_type = "bool";
                    }
                    else if (o_object.GetType() == typeof(bool[]))
                    {
                        s_type = "bool[]";
                    }
                    else if (o_object.GetType() == typeof(byte))
                    {
                        s_type = "byte";
                    }
                    else if (o_object.GetType() == typeof(byte[]))
                    {
                        s_type = "byte[]";
                    }
                    else if (o_object.GetType() == typeof(sbyte))
                    {
                        s_type = "sbyte";
                    }
                    else if (o_object.GetType() == typeof(sbyte[]))
                    {
                        s_type = "sbyte[]";
                    }
                    else if (o_object.GetType() == typeof(char))
                    {
                        s_type = "char";
                    }
                    else if (o_object.GetType() == typeof(char[]))
                    {
                        s_type = "char[]";
                    }
                    else if (o_object.GetType() == typeof(float))
                    {
                        s_type = "float";
                    }
                    else if (o_object.GetType() == typeof(float[]))
                    {
                        s_type = "float[]";
                    }
                    else if (o_object.GetType() == typeof(double))
                    {
                        s_type = "double";
                    }
                    else if (o_object.GetType() == typeof(double[]))
                    {
                        s_type = "double[]";
                    }
                    else if (o_object.GetType() == typeof(short))
                    {
                        s_type = "short";
                    }
                    else if (o_object.GetType() == typeof(short[]))
                    {
                        s_type = "short[]";
                    }
                    else if (o_object.GetType() == typeof(ushort))
                    {
                        s_type = "ushort";
                    }
                    else if (o_object.GetType() == typeof(ushort[]))
                    {
                        s_type = "ushort[]";
                    }
                    else if (o_object.GetType() == typeof(int))
                    {
                        s_type = "int";
                    }
                    else if (o_object.GetType() == typeof(int[]))
                    {
                        s_type = "int[]";
                    }
                    else if (o_object.GetType() == typeof(uint))
                    {
                        s_type = "uint";
                    }
                    else if (o_object.GetType() == typeof(uint[]))
                    {
                        s_type = "uint[]";
                    }
                    else if (o_object.GetType() == typeof(long))
                    {
                        s_type = "long";
                    }
                    else if (o_object.GetType() == typeof(long[]))
                    {
                        s_type = "long[]";
                    }
                    else if (o_object.GetType() == typeof(ulong))
                    {
                        s_type = "ulong";
                    }
                    else if (o_object.GetType() == typeof(ulong[]))
                    {
                        s_type = "ulong[]";
                    }
                    else if (o_object.GetType() == typeof(string))
                    {
                        s_type = "string";
                    }
                    else if ((o_object.GetType() == typeof(string[])) || (o_object.GetType() == typeof(string?[])))
                    {
                        s_type = "string[]";
                    }
                    else if ((o_object.GetType() == typeof(DateTime)) || (o_object.GetType() == typeof(DateTime?)))
                    {
                        s_type = "DateTime";
                    }
                    else if ((o_object.GetType() == typeof(DateTime[])) || (o_object.GetType() == typeof(DateTime?[])))
                    {
                        s_type = "DateTime[]";
                    }
                    else if (o_object.GetType() == typeof(decimal))
                    {
                        s_type = "decimal";
                    }
                    else if (o_object.GetType() == typeof(decimal[]))
                    {
                        s_type = "decimal[]";
                    }
                    else
                    {
                        if (this.MarshallingOverrideMessageType != null)
                        {
                            /* override message type with parameter, thus the type can be set generally from other systems with other programming languages */
                            s_type = this.MarshallingOverrideMessageType;
                        }
                        else
                        {
                            /* no primitive type -> set assembly qualified name */
                            s_type = o_object.GetType().AssemblyQualifiedName ?? "type AssemblyQualifiedName is null";
                        }
                    }
                }
                catch (Exception o_exc)
                {
                    throw new Exception("Could not marshall answer object: " + o_exc);
                }
            }
            else
            { /* no marshalling for network communication */
                /* read object data and serialize it to utf8 byte array */
                a_answerObjectBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(o_object);

                /* get assembly qualified type as string */
                s_type = o_object.GetType().AssemblyQualifiedName ?? "type AssemblyQualifiedName is null";
            }

            /* get data block length for network message */
            int i_dataBlockLength = ForestNET.Lib.Net.Msg.Message.CalculateDataBlockLength((this.BufferLength - i_decreaseForEncryption));
            int i_dataLength = a_answerObjectBytes.Length;

            /* calculate how many messages we need to transport answer object with network messages */
            i_messages = 1 + (i_dataLength / i_dataBlockLength);
            a_answerBytes = new byte[i_messages * (this.BufferLength - i_decreaseForEncryption)];
            int i_answerPointer = 0;

            /* split answer object data into several messages */
            for (k = 0; k < i_messages; k++)
            {
                /* create network message object with buffer length and encryption correction value */
                ForestNET.Lib.Net.Msg.Message o_message = new((this.BufferLength - i_decreaseForEncryption))
                {
                    /* set message information, like message box id, amount of messages, message number and type */
                    MessageBoxId = 1,
                    MessageAmount = i_messages,
                    MessageNumber = k + 1,
                    Type = s_type
                };

                /* create byte array for part of object data */
                byte[] a_data = new byte[i_dataBlockLength];
                int j;

                /* iterate all data bytes until we reached message data block length */
                for (j = 0; j < i_dataBlockLength; j++)
                {
                    /* reached last byte? -> break */
                    if (j + (k * i_dataBlockLength) >= a_answerObjectBytes.Length)
                    {
                        break;
                    }

                    /* copy data byte to byte array */
                    a_data[j] = a_answerObjectBytes[j + (k * i_dataBlockLength)];
                }

                /* give byte array as data part to network message object */
                o_message.Data = a_data;

                /* part of object data may not need complete data block length, especially the last message or just one message, so this is not obsolete because the for loop before could be break before j == i_dataBlockLength - 1 */
                if (j != i_dataBlockLength - 1)
                {
                    /* update message data length */
                    o_message.DataLength = j;
                }

                /* get message byte array */
                a_data = o_message.GetByteArrayFromMessage();

                /* copy message byte array to byte array of answer object */
                for (j = 0; j < a_answerBytes.Length; j++)
                {
                    a_answerBytes[i_answerPointer++] = a_data[j];
                }
            }

            /* encrypt answer bytes if encryption is active */
            if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
            {
                a_answerBytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_answerBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
            }
            else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
            {
                a_answerBytes = ForestNET.Lib.Cryptography.Encrypt_AES_GCM(a_answerBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
            }
            else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
            {
                a_answerBytes = this.Cryptography?.Encrypt(a_answerBytes);
            }

            if ((a_answerBytes != null) && (a_answerBytes.Length > 0))
            {
                ForestNET.Lib.Global.ILogFine("Amount bytes protocol, starting");

                /* use AmountBytesProtocol to send to other communication side how many bytes are to be expected */
                await this.AmountBytesProtocol(a_answerBytes.Length);

                ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, finished");

                ForestNET.Lib.Global.ILogFine("Sending answer message data, amount of bytes: " + a_answerBytes.Length);

                /* sending answer object within message context */
                await this.SendBytes(a_answerBytes);
            }

            /* with asymmetric encryption we have to close our ssl socket ourselves */
            if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.ASYMMETRIC) && (this.ReceivingSocket?.Connected ?? false))
            {
                this.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                this.ReceivingSocket?.Close();
            }
        }
    }
}