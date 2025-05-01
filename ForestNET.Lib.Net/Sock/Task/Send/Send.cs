namespace ForestNET.Lib.Net.Sock.Task.Send
{
    /// <summary>
    /// Generic task class to send network traffic over a socket instance. Several methods supporting UDP and TCP, sending data in combination with a message box or multiple message boxes.
    /// </summary>
    public class Send : ForestNET.Lib.Net.Sock.Task.Task
    {
        /// <summary>
        /// Parameterless constructor for TCP RunServer method
        /// </summary>
        public Send() : base(ForestNET.Lib.Net.Sock.Type.TCP_TLS_CLIENT)
        {

        }

        /// <summary>
        /// Creating sending socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Send(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds) :
            base(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds)
        {

        }

        /// <summary>
        /// Creating sending socket task instance with all it's parameters and settings
        /// </summary>
        /// <param name="p_e_communicationType">specifies communication type of socket task</param>
        /// <param name="p_e_communicationCardinality">specifies communication cardinality of socket task</param>
        /// <param name="p_i_queueTimeoutMilliseconds">determine timeout in milliseconds for sending/receiving bytes</param>
        /// <param name="p_e_communicationSecurity">specifies communication security of socket task</param>
        /// <exception cref="ArgumentException">invalid timeout value for queue</exception>
        public Send(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity) :
            base(p_e_communicationType, p_e_communicationCardinality, p_i_queueTimeoutMilliseconds, p_e_communicationSecurity)
        {

        }

        /// <summary>
        /// Creating sending socket task instance with all it's parameters and settings
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
        public Send(ForestNET.Lib.Net.Sock.Com.Type p_e_communicationType, ForestNET.Lib.Net.Sock.Com.Cardinality? p_e_communicationCardinality, int p_i_queueTimeoutMilliseconds, ForestNET.Lib.Net.Sock.Com.Security? p_e_communicationSecurity, bool p_b_useMarshalling, int p_i_marshallingDataLengthInBytes, bool p_b_marshallingUseProperties, string? p_s_marshallingOverrideMessageType, bool p_b_marshallingSystemUsesLittleEndian) :
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
        /// runTask method of sending socket task which can always vary depending on the implementation. Supporting in this class UDP_SEND, UDP_SEND_WITH_ACK, TCP_SEND and TCP_SEND_WITH_ANSWER.
        /// </summary>
        /// <exception cref="Exception">any exception of implementation that could happen will be caught by abstract Task class, see details in protocol methods in ForestNET.Lib.Net.Sock.Task.send.Send</exception>
        public override async System.Threading.Tasks.Task RunTask()
        {
            /* flag if data has been sent within socket task */
            bool b_dataSend = false;

            if ((this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND) || (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK))
            {
                /* send UDP packets */
                b_dataSend = await this.UDPSend();
            }
            else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND)
            {
                /* check if we want to use tcp socket for bidirectional communication */
                if (this.CommunicationCardinality == ForestNET.Lib.Net.Sock.Com.Cardinality.EqualBidirectional)
                {
                    await this.TCPBidirectional();
                }
                else
                {
                    /* check all message boxes of socket task */
                    foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.MessageBoxes ?? [])
                    {
                        /* get message of current message box */
                        ForestNET.Lib.Net.Msg.Message? o_message = o_messageBox.CurrentMessage();

                        /* if we got a message we want to send it */
                        if (o_message != null)
                        {
                            if (this.ObjectTransmission)
                            {
                                /* sending TCP packets, but a whole object, so several messages of a message box until the object has been transferred */
                                b_dataSend = await this.TCPSendObjectTransmission(o_message, o_messageBox);
                            }
                            else
                            {
                                /* send TCP packet: just dequeue one message of current message box */
                                o_message = o_messageBox.DequeueMessage();

                                if (o_message != null)
                                {
                                    /* send message with TCP protocol */
                                    b_dataSend = await this.TCPSend(o_message);
                                }
                                else
                                {
                                    throw new ArgumentNullException("Could not dequeue message, result is null");
                                }
                            }

                            /* break for loop here, so the next send will happen with next thread cycle */
                            break;
                        }
                    }
                }
            }
            else if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.TCP_SEND_WITH_ANSWER)
            {
                /* send TCP packet and receive answer immediately with answerMessageBox property */
                b_dataSend = await this.TCPSendWithAnswer();
            }

            /* if no data has been sent we wait queue timeout length  */
            if (!b_dataSend)
            {
                ForestNET.Lib.Global.ILogFinest("no data send, timeout for '" + this.QueueTimeoutMilliseconds + "' milliseconds");

                /* wait queue timeout length to enqueue message object again */
                await System.Threading.Tasks.Task.Delay(this.QueueTimeoutMilliseconds, this.CancellationToken ?? default);
            }
        }

        /// <summary>
        /// Method for sending a UDP datagram packet, optional with receiving an acknowledge(ACK) over UDP which is not UDP standard but could be useful sometimes
        /// </summary>
        /// <exception cref="ArgumentNullException">message could not be dequeued from message box</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        private async System.Threading.Tasks.Task<bool> UDPSend()
        {
            /* check if we have a valid UDP socket */
            if (this.UDPSocket == null)
            {
                throw new ArgumentNullException("Datagram socket is null");
            }

            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* flag if data has been sent within socket task */
            bool b_dataSend = false;

            /* check all message boxes of socket task */
            foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.MessageBoxes)
            {
                /* get message of current message box */
                ForestNET.Lib.Net.Msg.Message? o_message = o_messageBox.CurrentMessage();

                /* if we got a message we want to send it */
                if (o_message != null)
                {
                    /* just dequeue one message of current message box */
                    o_message = o_messageBox.DequeueMessage();

                    if (o_message != null)
                    {
                        /* convert message into byte array */
                        byte[] a_bytes = o_message.GetByteArrayFromMessage();

                        ForestNET.Lib.Global.ILogFine("Send message to[" + this.UDPSocket.RemoteEndPoint + "]: message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + a_bytes.Length + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_messageBox.MessageBoxId + "]");

                        /* debug sending message if b_debugNetworkTrafficOn is set to true */
                        this.DebugMessage(a_bytes, o_message);

                        /* encrypt sending bytes if encryption is active */
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

                        /* sending bytes via UDP */
                        _ = await this.UDPSocket.SendAsync(a_bytes, System.Net.Sockets.SocketFlags.None, new System.Threading.CancellationTokenSource(this.UDPSocket.SendTimeout).Token);

                        /* update flag */
                        b_dataSend = true;

                        /* communication type requires to receive an acknowledge(ACK) over UDP */
                        if (this.CommunicationType == ForestNET.Lib.Net.Sock.Com.Type.UDP_SEND_WITH_ACK)
                        {
                            /* execute method to receive an acknowledge(ACK) */
                            if (!await this.ReceiveUdpAck())
                            {
                                ForestNET.Lib.Global.ILogWarning("Could not receive ACK");
                            }
                        }

                        /* break for loop here, so the next send will happen with next thread cycle */
                        break;
                    }
                    else
                    {
                        throw new ArgumentNullException("Could not dequeue message, result is null");
                    }
                }
            }

            return b_dataSend;
        }

        /// <summary>
        /// Receive UDP ACK using own datagram source address and source port. All exceptions will be handled within this method and are not thrown to parent methods.
        /// </summary>
        /// <returns>true - ACK has been received so communication is successful, false - nothing received so something went wrong</returns>
        private async System.Threading.Tasks.Task<bool> ReceiveUdpAck()
        {
            bool b_receivedACK = false;

            try
            {
                /* check UDP socket */
                if (this.UDPSocket == null)
                {
                    throw new ArgumentException("UDP socket where we sent data is null");
                }

                /* check UDP socket remote address */
                if (this.UDPSocket.RemoteEndPoint == null)
                {
                    throw new ArgumentException("UDP socket remote address where we receive data is null");
                }

                /* check UDP socket local address */
                if (this.UDPSocket.LocalEndPoint == null)
                {
                    throw new ArgumentException("UDP socket remote address where we listen for data is null");
                }

                System.Net.EndPoint? o_recvPoint = this.UDPSocket.RemoteEndPoint;
                System.Net.EndPoint? o_listenPoint = this.UDPSocket.LocalEndPoint;

                ForestNET.Lib.Global.ILogFiner("SocketTaskSend, ReceiveUDPACK from " + this.UDPSocket.RemoteEndPoint);

                /* close current UDP socket, so we can use it to receive the ACK from the other communication side */
                this.UDPSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                this.UDPSocket?.Close();
                this.UDPSocket = null;

                int i_length = 1;

                /* check for length correction if encryption is active  */
                if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH))
                {
                    i_length = 29;
                }
                else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                {
                    i_length = 17;
                }

                /* create receive server and bind to configured address */
                System.Net.Sockets.Socket o_receiveSocket = new(o_recvPoint.AddressFamily, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                System.Net.Sockets.SocketOptionLevel o_socketOptionLevel = System.Net.Sockets.SocketOptionLevel.IP;

                if (o_recvPoint.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    o_socketOptionLevel = System.Net.Sockets.SocketOptionLevel.IPv6;
                }

                o_receiveSocket.SetSocketOption(o_socketOptionLevel, System.Net.Sockets.SocketOptionName.ReuseAddress, true);

                /* set receive buffer size */
                o_receiveSocket.ReceiveBufferSize = i_length;
                /* set socket timeout value */
                o_receiveSocket.ReceiveTimeout = this.UDPReceiveACKTimeoutMilliseconds;
                /* bind socket to address */
                o_receiveSocket.Bind(o_listenPoint);

                try
                {
                    /* byte array for receiving bytes from UDP datagram */
                    byte[] a_datagramPacketBytes = new byte[o_receiveSocket.ReceiveBufferSize];

                    ForestNET.Lib.Global.ILogFiner("wait to receive UDP ACK o_receiveSocket.ReceiveFromAsync() with " + o_listenPoint);

                    /* wait to receive UDP ACK */
                    await o_receiveSocket.ReceiveFromAsync(a_datagramPacketBytes, System.Net.Sockets.SocketFlags.None, o_recvPoint, new System.Threading.CancellationTokenSource(this.UDPReceiveACKTimeoutMilliseconds).Token);

                    /* check if received byte array length is greater than 0 */
                    if (a_datagramPacketBytes.Length < 1)
                    {
                        throw new ArgumentNullException("Could not receive ACK - length: " + a_datagramPacketBytes.Length + " < 1");
                    }

                    /* decrypt received bytes if encryption is active */
                    if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_HIGH)
                    {
                        a_datagramPacketBytes = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_datagramPacketBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY128BIT);
                    }
                    else if (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_HIGH)
                    {
                        a_datagramPacketBytes = ForestNET.Lib.Cryptography.Decrypt_AES_GCM(a_datagramPacketBytes, this.CommonSecretPassphrase ?? "", ForestNET.Lib.Cryptography.KEY256BIT);
                    }
                    else if ((this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_128_BIT_LOW) || (this.CommunicationSecurity == ForestNET.Lib.Net.Sock.Com.Security.SYMMETRIC_256_BIT_LOW))
                    {
                        a_datagramPacketBytes = this.Cryptography?.Decrypt(a_datagramPacketBytes) ?? [];
                    }

                    /* ACK variable */
                    byte by_ack = 0x00;

                    /* read UDP ACK */
                    by_ack = a_datagramPacketBytes[0];

                    /* check if received UDP ACK matches the ACK constant defined in ForestNET.Lib.Net.Sock.Task.Task class */
                    if (by_ack != ForestNET.Lib.Net.Sock.Task.Task.BY_ACK_BYTE)
                    {
                        throw new System.IO.IOException("Invalid ACK[" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { by_ack }, false).Trim() + "], must be [" + ForestNET.Lib.Helper.PrintByteArray(new byte[] { ForestNET.Lib.Net.Sock.Task.Task.BY_ACK_BYTE }, false).Trim() + "].");
                    }
                    else
                    {
                        /* update return value */
                        b_receivedACK = true;

                        ForestNET.Lib.Global.ILogFiner("received UDP ACK");
                    }
                }
                catch (OperationCanceledException o_exc)
                {
                    ForestNET.Lib.Global.ILogFiner("OperationCanceledException SocketTaskSend-ReceiveACK method: " + o_exc);
                }
                catch (System.IO.IOException o_exc)
                {
                    ForestNET.Lib.Global.LogException("IOException SocketTaskSend-receiveACK method: ", o_exc);
                }
                finally
                {
                    /* close UDP receive socket */
                    o_receiveSocket.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                    o_receiveSocket.Close();
                }
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception SocketTaskSend-receiveACK method: ", o_exc);
            }

            return b_receivedACK;
        }

        /// <summary>
        /// Method for sending and transferring an object with a TCP connection with messages
        /// </summary>
        /// <param name="p_o_message">first message of object which will be transferred</param>
        /// <param name="p_o_messageBox">message box where object is stored into several messages</param>
        /// <returns>true - object sent successfully, false - something went wrong</returns>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        private async System.Threading.Tasks.Task<bool> TCPSendObjectTransmission(ForestNET.Lib.Net.Msg.Message p_o_message, ForestNET.Lib.Net.Msg.MessageBox p_o_messageBox)
        {
            /* overall data length which will be transferred to other communication side */
            int i_transmissionDataLength = 0;
            /* array where all messages will be converted into bytes */
            List<byte> a_messagesBytes = [];

            try
            {
                /* help variables and amount of messages to be dequeued to transfer stored object */
                int i_messageAmount = p_o_message.MessageAmount;
                int i_messageNumber = 0;
                int i_dequeueCount = 0;

                /* gather all message bytes, based on message amount */
                do
                {
                    /* dequeue message from message box */
                    ForestNET.Lib.Net.Msg.Message? o_message = p_o_messageBox.DequeueMessage();

                    if (o_message != null)
                    { /* dequeue successful */
                        i_dequeueCount = 0;
                        i_messageNumber = o_message.MessageNumber;
                        byte[] a_messageBytes = o_message.GetByteArrayFromMessage();
                        i_transmissionDataLength += a_messageBytes.Length;

                        /* debug received message if b_debugNetworkTrafficOn is set to true */
                        this.DebugMessage(a_messageBytes, o_message);

                        /* gather bytes */
                        for (int i_cnt = 0; i_cnt < a_messageBytes.Length; i_cnt++)
                        {
                            a_messagesBytes.Add(a_messageBytes[i_cnt]);
                        }
                    }
                    else
                    { /* something went wrong with dequeuing message box */
                        int i_wait = 10;

                        ForestNET.Lib.Global.ILogFinest("Cannot dequeue message, timeout for '" + i_wait + "' milliseconds");

                        /* wait 10 milliseconds to dequeue message from message box object again */
                        await System.Threading.Tasks.Task.Delay(i_wait, this.CancellationToken ?? default);

                        i_dequeueCount++;

                        /* if 3000 times dequeue failed, something went wrong */
                        if (i_dequeueCount > (30000 / i_wait))
                        {
                            throw new InvalidOperationException("Could not dequeue message, result is null");
                        }
                    }
                } while ((i_messageAmount - i_messageNumber) != 0);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException("Exception Send-tcpSendObjectTransmission method: ", o_exc);
            }

            /* create byte array which will be send to other communication side over TCP */
            byte[] a_bytes = new byte[i_transmissionDataLength];
            int i = 0;

            /* copy all gathered message bytes into byte array */
            foreach (byte by_byte in a_messagesBytes)
            {
                a_bytes[i++] = by_byte;
            }

            /* encrypt sending bytes if encryption is active */
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

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, starting");

            /* use AmountBytesProtocol to send to other communication side how many bytes are to be expected */
            await this.AmountBytesProtocol(a_bytes.Length);

            ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, finished");

            ForestNET.Lib.Global.ILogFine("Sending data, amount of bytes: " + a_bytes.Length);

            /* send bytes over TCP */
            await this.SendBytes(a_bytes);

            return true;
        }

        /// <summary>
        /// Method for sending and transferring an object with a TCP connection with messages and receiving an answer with separate answer message box within socket task
        /// </summary>
        /// <exception cref="InvalidOperationException">amount of data is not a multiple of buffer length</exception>
        /// <exception cref="Exception">any other Exception, see below in other classes for details</exception>
        private async System.Threading.Tasks.Task<bool> TCPSendWithAnswer()
        {
            /* check if message boxes list is initiated */
            if (this.MessageBoxes == null)
            {
                throw new ArgumentNullException("Message boxes are null");
            }

            /* check if answer message boxes list is initiated */
            if (this.AnswerMessageBoxes == null)
            {
                throw new ArgumentNullException("Answer message boxes are null");
            }

            /* flag if data has been sent within socket task */
            bool b_dataSend = false;

            /* check all message boxes of socket task */
            foreach (ForestNET.Lib.Net.Msg.MessageBox o_messageBox in this.MessageBoxes)
            {
                /* get message of current message box */
                ForestNET.Lib.Net.Msg.Message? o_message = o_messageBox.CurrentMessage();

                /* if we got a message we want to send it */
                if (o_message != null)
                {
                    /* overall data length which will be transferred to other communication side */
                    int i_transmissionDataLength = 0;
                    /* array where all messages will be converted into bytes */
                    List<byte> a_messagesBytes = [];

                    try
                    {
                        /* help variables and amount of messages to be dequeued to transfer stored object */
                        int i_messageAmount = o_message.MessageAmount;
                        int i_messageNumber = 0;
                        int i_dequeueCount = 0;

                        /* gather all message bytes, based on message amount */
                        do
                        {
                            /* dequeue message from message box */
                            o_message = o_messageBox.DequeueMessage();

                            if (o_message != null)
                            { /* dequeue successful */
                                i_dequeueCount = 0;
                                i_messageNumber = o_message.MessageNumber;
                                byte[] a_messageBytes = o_message.GetByteArrayFromMessage();
                                i_transmissionDataLength += a_messageBytes.Length;

                                /* debug received message if b_debugNetworkTrafficOn is set to true */
                                this.DebugMessage(a_messageBytes, o_message);

                                /* gather bytes */
                                for (int i_cnt = 0; i_cnt < a_messageBytes.Length; i_cnt++)
                                {
                                    a_messagesBytes.Add(a_messageBytes[i_cnt]);
                                }
                            }
                            else
                            { /* something went wrong with dequeuing message box */
                                int i_wait = 10;

                                ForestNET.Lib.Global.ILogFinest("Cannot dequeue message, timeout for '" + i_wait + "' milliseconds");

                                /* wait 10 milliseconds to dequeue message from message box object again */
                                await System.Threading.Tasks.Task.Delay(i_wait, this.CancellationToken ?? default);

                                i_dequeueCount++;

                                /* if 3000 times dequeue failed, something went wrong */
                                if (i_dequeueCount > (30000 / i_wait))
                                {
                                    throw new ArgumentNullException("Could not dequeue message, result is null");
                                }
                            }
                        } while ((i_messageAmount - i_messageNumber) != 0);
                    }
                    catch (Exception o_exc)
                    {
                        ForestNET.Lib.Global.LogException("Exception Send-tcpSendWithAnswer method: ", o_exc);
                    }

                    /* create byte array which will be send to other communication side over TCP */
                    byte[] a_bytes = new byte[i_transmissionDataLength];
                    int i = 0;

                    /* copy all gathered message bytes into byte array */
                    foreach (byte by_byte in a_messagesBytes)
                    {
                        a_bytes[i++] = by_byte;
                    }

                    /* encrypt sending bytes if encryption is active */
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

                    ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, starting");

                    /* use AmountBytesProtocol to send to other communication side how many bytes are to be expected */
                    await this.AmountBytesProtocol(a_bytes.Length);

                    ForestNET.Lib.Global.ILogFiner("Amount bytes protocol, finished");

                    ForestNET.Lib.Global.ILogFine("Sending data, amount of bytes: " + a_bytes.Length);

                    /* send bytes over TCP */
                    await this.SendBytes(a_bytes);

                    /* update flag */
                    b_dataSend = true;

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

                    /* receive bytes for answer object */
                    byte[] a_receivedData = await this.ReceiveBytes(i_amountBytes) ?? [];

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
                        a_receivedData = this.Cryptography?.Decrypt(a_receivedData) ?? [];
                    }

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
                        if ((a_receivedData.Length % (this.BufferLength - i_decreaseForEncryption)) != 0)
                        {
                            throw new InvalidOperationException("Invalid amount of data[" + a_receivedData.Length + "]. Amount of data is not a multiple of buffer length[" + (this.BufferLength - i_decreaseForEncryption) + "].");
                        }
                    }

                    /* calculate how many messages we received for answer message object */
                    int i_messages = a_receivedData.Length / (this.BufferLength - i_decreaseForEncryption);

                    ForestNET.Lib.Global.ILogFine("Received answer message from[" + this.Socket?.SocketAddress + "]: messages[" + i_messages + "], length[" + a_receivedData.Length + "]");

                    for (i = 0; i < i_messages; i++)
                    {
                        byte[] a_messageData = new byte[(this.BufferLength - i_decreaseForEncryption)];

                        /* copy received answer data into message data array */
                        for (int i_bytePointer = 0; i_bytePointer < (this.BufferLength - i_decreaseForEncryption); i_bytePointer++)
                        {
                            a_messageData[i_bytePointer] = a_receivedData[i_bytePointer + (i * (this.BufferLength - i_decreaseForEncryption))];
                        }

                        /* create message object with message length and message data */
                        o_message = new ForestNET.Lib.Net.Msg.Message((this.BufferLength - i_decreaseForEncryption));
                        o_message.SetMessageFromByteArray(a_messageData);

                        ForestNET.Lib.Global.ILogFiner("Received answer message number[" + o_message.MessageNumber + "] of [" + o_message.MessageAmount + "] with length[" + o_message.MessageLength + "], data length[" + o_message.DataLength + "], data block length[" + o_message.DataBlockLength + "], message box id[" + o_message.MessageBoxId + "]");

                        /* debug received answer message if b_debugNetworkTrafficOn is set to true */
                        this.DebugMessage(a_messageData, o_message);

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
                            if (i_messageBoxId > this.AnswerMessageBoxes.Count)
                            {
                                throw new ArgumentException("Invalid receiving message. Message box id[" + i_messageBoxId + "] is not available.");
                            }

                            i_messageBoxId--;
                        }

                        /* enqueue answer message object to answer message box */
                        while (!this.AnswerMessageBoxes[i_messageBoxId].EnqueueMessage(o_message))
                        {
                            ForestNET.Lib.Global.ILogWarning("Could not enqueue message, timeout for '" + this.QueueTimeoutMilliseconds + "' milliseconds");

                            /* wait queue timeout length to enqueue answer message object again */
                            await System.Threading.Tasks.Task.Delay(this.QueueTimeoutMilliseconds, this.CancellationToken ?? default);
                        }
                        ;
                    }

                    /* break for loop here, so the next send will happen with next thread cycle */
                    break;
                }
            }

            return b_dataSend;
        }
    }
}