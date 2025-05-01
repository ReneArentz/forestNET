namespace ForestNET.Lib.Net.Sock.Com
{
    /// <summary>
    /// Class to run a task to watch a shared memory instance. In that way we can see how the fields of a class are changed in time.
    /// SharedMemoryTask can act as a receiver or sender, but not both at the same time. Changed fields will be enqueued or dequeued for network exchange working together with frameworks Communication Class.
    /// Each field if inherited class of shared memory instance will have it's own message box for sending/receiving.
    /// </summary>
    public class SharedMemoryTask
    {

        /* Fields */

        private readonly Communication o_communication;
        private readonly ISharedMemory? o_instanceToWatch;
        private readonly int i_timeoutMilliseconds;
        private readonly bool b_receive;
        private bool b_stop;
        private long l_lastCompleteRefresh;
        private readonly ForestNET.Lib.DateInterval? o_interval;
        private readonly bool b_bidirectional;
        private readonly bool b_marshallingWholeObject;
        private readonly CancellationTokenSource o_cancellationTokenSource;

        /* Properties */

        /* Methods */

        /// <summary>
        /// Create shared memory task with all necessary information. Is original source site. No interval object, sender will only send changed fields all the time.
        /// </summary>
        /// <param name="p_o_communication">communication object to enqueue or dequeue network data</param>
        /// <param name="p_o_instanceToWatch">instance object to be watched by this task</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for task instance - how long the shared memory task should wait after incoming/outgoing data cycle</param>
        /// <param name="p_e_communicationType">communication type enumeration to determine if shared memory task will send or receive network data</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched</exception>
        /// <exception cref="ArgumentNullException">communication object or instance to watch object parameter is null</exception>
        /// <exception cref="ArgumentException">invalid timeout value</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public SharedMemoryTask(Communication p_o_communication, ISharedMemory? p_o_instanceToWatch, int p_i_timeoutMilliseconds, Type p_e_communicationType) :
            this(p_o_communication, p_o_instanceToWatch, p_i_timeoutMilliseconds, p_e_communicationType, null)
        {

        }

        /// <summary>
        /// Create shared memory task with all necessary information. Is original source site
        /// </summary>
        /// <param name="p_o_communication">communication object to enqueue or dequeue network data</param>
        /// <param name="p_o_instanceToWatch">instance object to be watched by this task</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for task instance - how long the shared memory task should wait after incoming/outgoing data cycle</param>
        /// <param name="p_e_communicationType">communication type enumeration to determine if shared memory task will send or receive network data</param>
        /// <param name="p_o_interval">interval object for sender cycle. At the end of the interval all field values will be send to other communication side</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched</exception>
        /// <exception cref="ArgumentNullException">communication object or instance to watch object parameter is null</exception>
        /// <exception cref="ArgumentException">invalid timeout value</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public SharedMemoryTask(Communication p_o_communication, ISharedMemory? p_o_instanceToWatch, int p_i_timeoutMilliseconds, Type p_e_communicationType, ForestNET.Lib.DateInterval? p_o_interval) :
            this(p_o_communication, p_o_instanceToWatch, p_i_timeoutMilliseconds, p_e_communicationType, p_o_interval, false)
        {

        }

        /// <summary>
        /// Create shared memory task with all necessary information.
        /// </summary>
        /// <param name="p_o_communication">communication object to enqueue or dequeue network data</param>
        /// <param name="p_o_instanceToWatch">instance object to be watched by this task</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for task instance - how long the shared memory task should wait after incoming/outgoing data cycle</param>
        /// <param name="p_e_communicationType">communication type enumeration to determine if shared memory task will send or receive network data</param>
        /// <param name="p_o_interval">interval object for sender cycle. At the end of the interval all field values will be send to other communication side</param>
        /// <param name="p_b_bidirectional">true - indicate that this shared memory task communicate with the original source site of data, false - shared memory task is original source site</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched</exception>
        /// <exception cref="ArgumentNullException">communication object or instance to watch object parameter is null</exception>
        /// <exception cref="ArgumentException">invalid timeout value</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public SharedMemoryTask(Communication p_o_communication, ISharedMemory? p_o_instanceToWatch, int p_i_timeoutMilliseconds, Type p_e_communicationType, ForestNET.Lib.DateInterval? p_o_interval, bool p_b_bidirectional) :
            this(p_o_communication, p_o_instanceToWatch, p_i_timeoutMilliseconds, p_e_communicationType, p_o_interval, p_b_bidirectional, false)
        {

        }

        /// <summary>
        /// Create shared memory task with all necessary information.
        /// </summary>
        /// <param name="p_o_communication">communication object to enqueue or dequeue network data</param>
        /// <param name="p_o_instanceToWatch">instance object to be watched by this task</param>
        /// <param name="p_i_timeoutMilliseconds">set timeout value for task instance - how long the shared memory task should wait after incoming/outgoing data cycle</param>
        /// <param name="p_e_communicationType">communication type enumeration to determine if shared memory task will send or receive network data</param>
        /// <param name="p_o_interval">interval object for sender cycle. At the end of the interval all field values will be send to other communication side</param>
        /// <param name="p_b_bidirectional">true - indicate that this shared memory task communicate with the original source site of data, false - shared memory task is original source site</param>
        /// <param name="p_b_marshallingWholeObject">true - use marshalling methods for whole parameter object to transport data over network, especially with shared memory all fields will be transported within a cycle</param>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched</exception>
        /// <exception cref="ArgumentNullException">communication object or instance to watch object parameter is null</exception>
        /// <exception cref="ArgumentException">invalid timeout value</exception>
        /// <exception cref="MissingFieldException">a field does not exist</exception>
        /// <exception cref="AccessViolationException">cannot access field, must be public</exception>
        public SharedMemoryTask(Communication p_o_communication, ISharedMemory? p_o_instanceToWatch, int p_i_timeoutMilliseconds, Type p_e_communicationType, ForestNET.Lib.DateInterval? p_o_interval, bool p_b_bidirectional, bool p_b_marshallingWholeObject)
        {
            /* check timeout parameter */
            if (p_i_timeoutMilliseconds < 1)
            {
                throw new ArgumentException("Shared memory timeout must be at least '1' millisecond, but was set to '" + p_i_timeoutMilliseconds + "' millisecond(s)");
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "shared memory task timeout value in ms: '" + p_i_timeoutMilliseconds + "'");

            this.o_communication = p_o_communication ?? throw new ArgumentNullException(nameof(p_o_communication), "Communication parameter is null");
            this.o_instanceToWatch = p_o_instanceToWatch ?? throw new ArgumentNullException(nameof(p_o_instanceToWatch), "Instance to watch parameter is null");

            /* update mirror object for instance object to watch */
            this.o_instanceToWatch.UpdateMirror(false).Wait();
            /* update bidirectional mirror object for instance object to watch */
            this.o_instanceToWatch.UpdateMirror(true).Wait();

            /* set timeout parameter value */
            this.i_timeoutMilliseconds = p_i_timeoutMilliseconds;

            /* set receive flag based on communication type */
            if (
                p_e_communicationType == Type.UDP_RECEIVE ||
                p_e_communicationType == Type.UDP_RECEIVE_WITH_ACK ||
                p_e_communicationType == Type.TCP_RECEIVE
            )
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "shared memory task will receive data");
                this.b_receive = true;
            }
            else
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "shared memory task will send data");
                this.b_receive = false;
            }

            this.b_stop = false;
            this.l_lastCompleteRefresh = 0L;
            this.o_interval = null;

            /* set date interval object */
            if (p_o_interval != null)
            {
                ForestNET.Lib.Global.ILogConfig("\t" + "shared memory task date interval object set: '" + p_o_interval + "'");

                this.o_interval = p_o_interval;
            }

            ForestNET.Lib.Global.ILogConfig("\t" + "set flag to indicate that this shared memory task communicate with the original source site of data: '" + p_b_bidirectional + "'");

            /* set flag to indicate that this shared memory task communicate with the original source site of data */
            this.b_bidirectional = p_b_bidirectional;

            /* bidirectional side must not do first cycle and send all null values - collision with original source is avoided in that way */
            if (this.b_bidirectional)
            {
                this.l_lastCompleteRefresh = Environment.TickCount64;
            }

            /* set flag so that all fields within shared memory will be transported in one cycle */
            this.b_marshallingWholeObject = p_b_marshallingWholeObject;

            this.o_cancellationTokenSource = new();
        }

        /// <summary>
        /// This method stops the shared memory task.
        /// </summary>
        public void Stop()
        {
            this.b_stop = true;
            this.o_cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Core execution process method of shared memory task. Sending or receiving data based on receive flag.
        /// </summary>
        public async System.Threading.Tasks.Task RunTask()
        {
            try
            {
                ForestNET.Lib.Global.ILogConfig("shared memory task started");

                /* endless loop for our shared memory task instance */
                while (!this.b_stop)
                {
                    if (this.b_receive)
                    {
                        /* receiving data */
                        await this.ReceiverCycle();
                    }
                    else
                    {
                        /* sending data */
                        await this.SenderCycle();
                    }

                    /* execute task timeout */
                    await System.Threading.Tasks.Task.Delay(this.i_timeoutMilliseconds, this.o_cancellationTokenSource.Token);
                }
            }
            catch (TaskCanceledException o_exc)
            {
                /* log task cancellation, but do not abort or throw this exception further up */
                ForestNET.Lib.Global.ILogFine("shared memory task[" + this.GetType().FullName + "] was canceled: " + o_exc);
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }

            ForestNET.Lib.Global.ILogConfig("shared memory task stopped");
        }

        /// <summary>
        /// Sender execution process method.
        /// </summary>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched, exception within enqueuing network data</exception>
        /// <exception cref="IndexOutOfRangeException">if the field number is out of range (index < 0 || index >= size())</exception>
        /// <exception cref="AccessViolationException">field is not accessible</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="InvalidCastException">if the object is not null and is not assignable to the type T</exception>
        /// <exception cref="InvalidOperationException">communication is not running, wrong communication type or object does not implement Serializable interface</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter for enqueue method</exception>
        private async System.Threading.Tasks.Task SenderCycle()
        {
            /* check instance to watch variable */
            if (this.o_instanceToWatch == null)
            {
                throw new NullReferenceException("Instance to watch is null");
            }

            if (this.l_lastCompleteRefresh == 0L)
            { /* end of sender interval or first send of shared memory task -> will send all fields */
                ForestNET.Lib.Global.ILogFine("end of sender interval or first send of shared memory task -> will send all fields");

                /* update timestamp */
                this.l_lastCompleteRefresh = Environment.TickCount64;

                /* enqueue the whole object */
                if (this.b_marshallingWholeObject)
                {
                    ForestNET.Lib.Global.ILogFiner("enqueue whole object of '" + this.o_instanceToWatch.GetType().FullName + "'");

                    /* enqueue whole object */
                    while (!this.o_communication.Enqueue(
                        this.o_instanceToWatch
                    ))
                    {
                        ForestNET.Lib.Global.ILogFiner("enqueue whole object was not successful, wait timeout value '" + this.i_timeoutMilliseconds + " ms'");

                        /* if enqueue was not successful, wait timeout value */
                        await System.Threading.Tasks.Task.Delay(this.i_timeoutMilliseconds, this.o_cancellationTokenSource.Token);
                    }
                }
                else
                {
                    /* iterate all fields */
                    for (int i = 0; i < this.o_instanceToWatch.AmountFields(); i++)
                    {
                        /* get field name */
                        string s_field = this.o_instanceToWatch.ReturnFieldName(i + 1) ?? "";

                        ForestNET.Lib.Global.ILogFiner("enqueue field '" + s_field + "'");

                        while (!this.o_communication.Enqueue(
                            this.o_instanceToWatch.ReturnFieldNumber(s_field), await this.o_instanceToWatch.GetField(s_field, true)
                        ))
                        {
                            ForestNET.Lib.Global.ILogFiner("enqueue was not successful, wait timeout value '" + this.i_timeoutMilliseconds + " ms'");

                            /* if enqueue was not successful, wait timeout value */
                            await System.Threading.Tasks.Task.Delay(this.i_timeoutMilliseconds, this.o_cancellationTokenSource.Token);
                        }
                    }
                }

                ForestNET.Lib.Global.ILogFine("update (bidirectional) mirror object for instance object to watch");

                /* update (bidirectional) mirror object for instance object to watch */
                this.o_instanceToWatch?.UpdateMirror(this.b_bidirectional);
            }
            else
            { /* usual sender cycle */
                /* get changed fields */
                List<string> a_changedFields = await this.o_instanceToWatch.GetChangedFields(true, this.b_bidirectional);

                /* check if we have any changes fields */
                if (a_changedFields.Count > 0)
                {
                    ForestNET.Lib.Global.ILogFine(a_changedFields.Count + " fields have been changed");

                    /* enqueue the whole object if fields have changed */
                    if (this.b_marshallingWholeObject)
                    {
                        ForestNET.Lib.Global.ILogFiner("enqueue whole object of '" + this.o_instanceToWatch?.GetType().FullName + "'");

                        /* enqueue whole object */
                        while (!this.o_communication.Enqueue(
                            this.o_instanceToWatch ?? throw new NullReferenceException("Instance to watch is null")
                        ))
                        {
                            ForestNET.Lib.Global.ILogFiner("enqueue whole object was not successful, wait timeout value '" + this.i_timeoutMilliseconds + " ms'");

                            /* if enqueue was not successful, wait timeout value */
                            await System.Threading.Tasks.Task.Delay(this.i_timeoutMilliseconds, this.o_cancellationTokenSource.Token);
                        }
                    }
                    else
                    {
                        /* iterate only changed fields */
                        foreach (string s_field in a_changedFields)
                        {
                            ForestNET.Lib.Global.ILogFiner("enqueue field '" + s_field + "'");

                            /* enqueue field data, using field number as message box number */
                            while (!this.o_communication.Enqueue(
                                this.o_instanceToWatch.ReturnFieldNumber(s_field), await this.o_instanceToWatch.GetField(s_field, true)
                            ))
                            {
                                ForestNET.Lib.Global.ILogFiner("enqueue was not successful, wait timeout value '" + this.i_timeoutMilliseconds + " ms'");

                                /* if enqueue was not successful, wait timeout value */
                                await System.Threading.Tasks.Task.Delay(this.i_timeoutMilliseconds, this.o_cancellationTokenSource.Token);
                            }
                        }
                    }
                }
                else
                {
                    ForestNET.Lib.Global.ILogFine("no fields have been changed");
                }
            }

            /* if there is an interval for sender and timestamp is not null */
            if ((this.o_interval != null) && (this.l_lastCompleteRefresh != 0L))
            {
                /* if interval has expired */
                if (Environment.TickCount64 > (this.l_lastCompleteRefresh + this.o_interval.ToDuration()))
                {
                    /* set timestamp to zero -> end of sender interval, next cycle will send all fields */
                    this.l_lastCompleteRefresh = 0L;
                }
            }
        }

        /// <summary>
        /// Receiver execution process method.
        /// </summary>
        /// <exception cref="Exception">any exception which can happen during creating a new instance of instance to be watched, exception within dequeuing network data</exception>
        /// <exception cref="IndexOutOfRangeException">if the field number is out of range (index < 0 || index >= size())</exception>
        /// <exception cref="AccessViolationException">field is not accessible</exception>
        /// <exception cref="MissingFieldException">field does not exist</exception>
        /// <exception cref="InvalidCastException">if the object is not null and is not assignable to the type T</exception>
        /// <exception cref="InvalidOperationException">communication is not running or wrong communication type</exception>
        /// <exception cref="ArgumentException">invalid message box id parameter for enqueue method</exception>
        private async System.Threading.Tasks.Task ReceiverCycle()
        {
            /* check instance to watch variable */
            if (this.o_instanceToWatch == null)
            {
                throw new NullReferenceException("Instance to watch is null");
            }

            /* receive whole object */
            if (this.b_marshallingWholeObject)
            {
                /* dequeue shared memory object from communication message box */
                ISharedMemory? o_receivedSharedMemory = (ISharedMemory?)this.o_communication.Dequeue();

                if (o_receivedSharedMemory != null)
                {
                    if (this.b_bidirectional)
                    {
                        /* update bidirectional mirror field */
                        await this.o_instanceToWatch.UpdateWholeMirror(o_receivedSharedMemory, false);
                    }
                    else
                    {
                        /* update mirror field */
                        await this.o_instanceToWatch.UpdateWholeMirror(o_receivedSharedMemory, true);
                    }
                }
            }
            else
            { /* check each message box for the corresponding field  */
                /* iterate all fields in each receiver each cycle */
                for (int i = 0; i < this.o_instanceToWatch.AmountFields(); i++)
                {
                    Object? o_object;

                    ForestNET.Lib.Global.ILogFiner("dequeue object from communication message box '" + (i + 1) + "'");

                    /* dequeue object from communication message box */
                    o_object = this.o_communication.Dequeue(i + 1);

                    if (o_object != null)
                    {
                        /* get field name by message box number */
                        string s_field = this.o_instanceToWatch.ReturnFieldName(i + 1);

                        ForestNET.Lib.Global.ILogFiner("set received field value for field '" + s_field + "'");

                        /* set received field value */
                        await this.o_instanceToWatch.SetField(s_field, o_object);

                        ForestNET.Lib.Global.ILogFiner("update" + ((this.b_bidirectional) ? " bidirectional " : " ") + "mirror field");

                        if (this.b_bidirectional)
                        {
                            /* update bidirectional mirror field */
                            await this.o_instanceToWatch.UpdateMirrorField(s_field, false);
                        }
                        else
                        {
                            /* update mirror field */
                            await this.o_instanceToWatch.UpdateMirrorField(s_field, true);
                        }
                    }
                }
            }
        }
    }
}