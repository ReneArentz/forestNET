namespace ForestNET.Tests.Net.Sock
{
    public class TCPTest : ForestNET.Lib.Net.Sock.Task.Task
    {
        public int TaskNumber { get; set; } = 0;
        public string? BigFilePath { get; set; }

        /* parameterless constructor for TCP RunServer method */
        public TCPTest() : base(ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
        {

        }

        public TCPTest(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
        {

        }

        public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
        {
            this.CloneBasicProperties(p_o_sourceTask);

            /* ignore exceptions if a property of source task has no valid value, we will keep it null */
            try { this.TaskNumber = ((TCPTest)p_o_sourceTask).TaskNumber; } catch (Exception) { /* NOP */ }
            try { this.BigFilePath = ((TCPTest)p_o_sourceTask).BigFilePath; } catch (Exception) { /* NOP */ }
        }

        public override async Task RunTask()
        {
            if (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_SERVER)
            {
                if (TaskNumber == 1)
                {
                    await this.RunServer01();
                }
                else if (TaskNumber == 2)
                {
                    await this.RunServer02();
                }
                else if (TaskNumber == 3)
                {
                    await this.RunServer03();
                }
            }
            else if (this.Type == ForestNET.Lib.Net.Sock.Type.TCP_CLIENT)
            {
                if (TaskNumber == 1)
                {
                    await this.RunClient01();
                }
                else if (TaskNumber == 2)
                {
                    await this.RunClient02();
                }
                else if (TaskNumber == 3)
                {
                    await this.RunClient03();
                }
            }
        }

        public async Task RunServer01()
        {
            try
            {
                ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming socket communication with " + this.ReceivingSocket?.RemoteEndPoint);

                int i_amountBytes = await this.AmountBytesProtocol();
                ForestNET.Lib.Global.ILog("S" + "\t" + "Amount bytes: " + i_amountBytes);

                /* ------------------------------------------------------ */

                byte[] a_receivedData = await this.ReceiveBytes(i_amountBytes) ?? [];
                string s_message = System.Text.Encoding.UTF8.GetString(a_receivedData);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Received data: '" + s_message + "'");

                Assert.That(a_receivedData, Has.Length.EqualTo(13), "data length is not '13', but '" + a_receivedData.Length + "'");
                Assert.That(s_message, Is.EqualTo("Hello World!!"), "data content is not 'Hello World!!', but '" + s_message + "'");

                /* ------------------------------------------------------ */

                await this.SendACK();

                ForestNET.Lib.Global.ILog("S" + "\t" + "Socket communication closed");

                this.ReceivingSocket?.Shutdown(System.Net.Sockets.SocketShutdown.Both);
                this.ReceivingSocket?.Close();
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public async Task RunClient01()
        {
            try
            {
                ForestNET.Lib.Global.ILog("C" + "\t" + "Starting socket communication");

                string s_message = "Hello World!!";

                int i_foo = await this.AmountBytesProtocol(s_message.Length);

                /* ------------------------------------------------------ */

                await this.SendBytes(System.Text.Encoding.UTF8.GetBytes(s_message));
                ForestNET.Lib.Global.ILog("C" + "\t" + "Sended data, amount of bytes: " + s_message.Length);

                /* ------------------------------------------------------ */

                await this.ReceiveACK();

                ForestNET.Lib.Global.ILog("C" + "\t" + "Socket communication finished");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public async Task RunServer02()
        {
            try
            {
                ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming socket communication with " + this.ReceivingSocket?.RemoteEndPoint);

                int i_amountBytes = await this.AmountBytesProtocol();
                ForestNET.Lib.Global.ILog("S" + "\t" + "Amount bytes: " + i_amountBytes);

                /* ------------------------------------------------------ */

                byte[] a_receivedData = await this.ReceiveBytes(i_amountBytes) ?? [];
                string? s_message = (string?)ForestNET.Lib.Net.Msg.Marshall.UnmarshallObject(typeof(string), a_receivedData);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Received data: '" + s_message + "'");

                Assert.That(a_receivedData, Has.Length.EqualTo(21), "data length is not '21', but '" + a_receivedData.Length + "'");
                Assert.That(s_message, Is.EqualTo("Hello World!!"), "data content is not 'Hello World!!', but '" + s_message + "'");

                /* ------------------------------------------------------ */

                await this.SendACK();

                ForestNET.Lib.Global.ILog("S" + "\t" + "Socket communication closed");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public async Task RunClient02()
        {
            try
            {
                ForestNET.Lib.Global.ILog("C" + "\t" + "Starting socket communication");

                string s_message = "Hello World!!";
                byte[] a_data = ForestNET.Lib.Net.Msg.Marshall.MarshallObject(s_message);

                int i_foo = await this.AmountBytesProtocol(a_data.Length);

                /* ------------------------------------------------------ */

                await this.SendBytes(a_data);
                ForestNET.Lib.Global.ILog("C" + "\t" + "Sended data, amount of bytes: " + a_data.Length);

                /* ------------------------------------------------------ */

                await this.ReceiveACK();

                ForestNET.Lib.Global.ILog("C" + "\t" + "Socket communication finished");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public async Task RunServer03()
        {
            try
            {
                ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming socket communication with " + this.ReceivingSocket?.RemoteEndPoint);

                int i_amountBytes = await this.AmountBytesProtocol();
                ForestNET.Lib.Global.ILog("S" + "\t" + "Amount bytes: " + i_amountBytes);

                /* ------------------------------------------------------ */

                ForestNET.Lib.Global.ILog("S" + "\t" + "Start receiving data");
                byte[] a_receivedData = await this.ReceiveBytes(i_amountBytes) ?? [];
                ForestNET.Lib.Global.ILog("S" + "\t" + "Received file with length[" + a_receivedData.Length + "]");

                using (FileStream o_fileStream = new(this.BigFilePath ?? throw new NullReferenceException("big file path is null"), FileMode.Create, FileAccess.Write))
                {
                    o_fileStream.Write(a_receivedData, 0, a_receivedData.Length);
                }

                ForestNET.Lib.Global.ILog("S" + "\t" + "Wrote bytes into file");

                /* ------------------------------------------------------ */

                await this.SendACK();

                ForestNET.Lib.Global.ILog("S" + "\t" + "Socket communication closed");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        public async Task RunClient03()
        {
            try
            {
                ForestNET.Lib.Global.ILog("C" + "\t" + "Starting socket communication");

                byte[] a_data = File.ReadAllBytes(this.BigFilePath ?? throw new NullReferenceException("big file path is null"));

                int i_amountBytes = await this.AmountBytesProtocol(a_data.Length);
                ForestNET.Lib.Global.ILog("C" + "\t" + "Amount bytes: " + i_amountBytes);

                /* ------------------------------------------------------ */

                ForestNET.Lib.Global.ILog("C" + "\t" + "Start sending data");
                await this.SendBytes(a_data);
                ForestNET.Lib.Global.ILog("C" + "\t" + "Sended data, amount of bytes: " + a_data.Length);

                /* ------------------------------------------------------ */

                await this.ReceiveACK();

                ForestNET.Lib.Global.ILog("C" + "\t" + "Socket communication finished");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
