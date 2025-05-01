namespace ForestNET.Tests.Net.Sock
{
    public class UDPTest : ForestNET.Lib.Net.Sock.Task.Task
    {
        public int TaskNumber { get; set; } = 0;

        /* parameterless constructor for UDP RunServer method */
        public UDPTest() : base(ForestNET.Lib.Net.Sock.Type.UDP_SERVER)
        {

        }

        public UDPTest(ForestNET.Lib.Net.Sock.Type p_e_type) : base(p_e_type)
        {

        }

        public override void CloneFromOtherTask(ForestNET.Lib.Net.Sock.Task.Task p_o_sourceTask)
        {
            this.CloneBasicProperties(p_o_sourceTask);

            /* ignore exceptions if a property of source task has no valid value, we will keep it null */
            try { this.TaskNumber = ((UDPTest)p_o_sourceTask).TaskNumber; } catch (Exception) { /* NOP */ }
        }

        public override async Task RunTask()
        {
            if (this.Type == ForestNET.Lib.Net.Sock.Type.UDP_SERVER)
            {
                if (TaskNumber == 1)
                {
                    await this.RunServer01();
                }
                else if (TaskNumber == 2)
                {
                    await this.RunServer02();
                }
            }
            else if (this.Type == ForestNET.Lib.Net.Sock.Type.UDP_CLIENT)
            {
                if (TaskNumber == 1)
                {
                    await this.RunClient01();
                }
                else if (TaskNumber == 2)
                {
                    await this.RunClient02();
                }
            }
        }

        public async Task RunServer01()
        {
            try
            {
                if (this.UDPSourceAddress == null)
                {
                    throw new Exception("UDP source address is null");
                }

                ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming datagram packet communication with " + this.UDPSourceAddress);
                this.DatagramBytes = UDPDatagramBytesTrimEnd(this.DatagramBytes);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Data length: " + this.DatagramBytes?.Length);
                string s_foo = System.Text.Encoding.UTF8.GetString(this.DatagramBytes ?? []).Trim();
                ForestNET.Lib.Global.ILog("S" + "\t" + "Data: " + s_foo);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Datagram packet communication closed");

                Assert.That(s_foo, Has.Length.EqualTo(13), "data length is not '13', but '" + s_foo.Length + "'");
                Assert.That(s_foo, Is.EqualTo("Hello World!!"), "data content is not 'Hello World!!', but '" + s_foo + "'");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            await DoNoting();
        }

        public async Task RunClient01()
        {
            try
            {
                if (this.UDPSocket == null)
                {
                    throw new Exception("UDP socket is null");
                }

                if (this.UDPSocket.LocalEndPoint == null)
                {
                    throw new Exception("UDP socket local end point is null");
                }

                if (this.UDPSocket.RemoteEndPoint == null)
                {
                    throw new Exception("UDP socket remote end point is null");
                }

                ForestNET.Lib.Global.ILog("C" + "\t" + "Send outgoing datagram packet with " + this.UDPSocket.RemoteEndPoint.ToString());

                string s_message = "Hello World!!";
                byte[] a_bytes = System.Text.Encoding.UTF8.GetBytes(s_message);

                ForestNET.Lib.Global.ILog("C" + "\t" + "Data length: " + a_bytes.Length);
                ForestNET.Lib.Global.ILog("C" + "\t" + "Data: " + System.Text.Encoding.UTF8.GetString(a_bytes));

                int i_bytesSend = await this.UDPSocket.SendAsync(a_bytes, System.Net.Sockets.SocketFlags.None, new System.Threading.CancellationTokenSource(this.UDPSocket.SendTimeout).Token);

                ForestNET.Lib.Global.ILog("C" + "\t" + "'" + i_bytesSend + " bytes' sended");

                ForestNET.Lib.Global.ILog("C" + "\t" + "Datagram packet communication closed");
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
                if (this.UDPSourceAddress == null)
                {
                    throw new Exception("UDP source address is null");
                }

                ForestNET.Lib.Global.ILog("S" + "\t" + "Handle incoming datagram packet communication with " + this.UDPSourceAddress);
                this.DatagramBytes = UDPDatagramBytesTrimEnd(this.DatagramBytes);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Data length: " + this.DatagramBytes?.Length);
                string? s_foo = (string?)ForestNET.Lib.Net.Msg.Marshall.UnmarshallObject(typeof(string), this.DatagramBytes ?? []);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Data: " + s_foo);
                ForestNET.Lib.Global.ILog("S" + "\t" + "Datagram packet communication closed");

                Assert.That(this.DatagramBytes, Has.Length.EqualTo(21), "data length is not '21', but '" + this.DatagramBytes?.Length + "'");
                Assert.That(s_foo, Is.EqualTo("Hello World!!"), "data content is not 'Hello World!!', but '" + s_foo + "'");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }

            await DoNoting();
        }

        public async Task RunClient02()
        {
            try
            {
                if (this.UDPSocket == null)
                {
                    throw new Exception("UDP socket is null");
                }

                if (this.UDPSocket.LocalEndPoint == null)
                {
                    throw new Exception("UDP socket local end point is null");
                }

                if (this.UDPSocket.RemoteEndPoint == null)
                {
                    throw new Exception("UDP socket remote end point is null");
                }

                ForestNET.Lib.Global.ILog("C" + "\t" + "Send outgoing datagram packet with " + this.UDPSocket.RemoteEndPoint.ToString());

                string s_message = "Hello World!!";
                byte[] a_bytes = ForestNET.Lib.Net.Msg.Marshall.MarshallObject(s_message);

                ForestNET.Lib.Global.ILog("C" + "\t" + "Data length: " + a_bytes.Length);

                int i_bytesSend = await this.UDPSocket.SendAsync(a_bytes, System.Net.Sockets.SocketFlags.None, new System.Threading.CancellationTokenSource(this.UDPSocket.SendTimeout).Token);

                ForestNET.Lib.Global.ILog("C" + "\t" + "'" + i_bytesSend + " bytes' sended");

                ForestNET.Lib.Global.ILog("C" + "\t" + "Datagram packet communication closed");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
