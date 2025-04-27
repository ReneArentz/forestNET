namespace ForestNET.Tests.Net.Msg
{
    public class MessageBoxMarshallingUnitTest
    {
        [Test]
        public void TestMessageBoxMarshalling()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                ForestNET.Lib.Net.Msg.MessageBox o_messageBox = new(1, 1500);

                bool b_marshallingObjectsUseProperties = true; /* our message objects only have properties, so it will the parameter will alwasy be true */
                int i_dataLengthInBytes = 1;
                bool b_useProperties = false;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, false);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 2;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, true);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 3;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, false);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 4;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, true);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 1;
                b_useProperties = true;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, true);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 2;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, false);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 3;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, true);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);

                i_dataLengthInBytes = 4;

                TestMessageBoxMarshallingObjects(o_messageBox, i_dataLengthInBytes, b_marshallingObjectsUseProperties, false);
                TestMessageBoxMarshallingPrimitives(o_messageBox, i_dataLengthInBytes, b_useProperties);
                TestMessageBoxMarshallingArrays(o_messageBox, i_dataLengthInBytes, b_useProperties);
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void TestMessageBoxMarshallingObjects(ForestNET.Lib.Net.Msg.MessageBox p_o_messageBox, int p_i_dataLengthInBytes, bool p_b_useProperties, bool p_b_overrideTypeName)
        {
            MessageObject o_messageObject = new();
            o_messageObject.InitAll();

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_messageObject, p_i_dataLengthInBytes, p_b_useProperties, p_b_overrideTypeName ? "ForestNET.Tests.Net.Msg.MessageObject" : null))
            {
                throw new Exception("Could not enqueue object");
            }

            o_messageObject.EmptyAll();

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_messageObject, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            SmallMessageObject o_smallMessageObject = new();
            o_smallMessageObject.InitAll();

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_smallMessageObject, p_i_dataLengthInBytes, p_b_useProperties, p_b_overrideTypeName ? "SmallMessageObject" : null))
            {
                throw new Exception("Could not enqueue object");
            }

            o_smallMessageObject.EmptyAll();

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_smallMessageObject, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            MessageObject o_returnMessageObject = (MessageObject)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            MessageObject o_returnMessageObjectEmpty = (MessageObject)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            SmallMessageObject o_returnSmallMessageObject = (SmallMessageObject)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued small message object is null"));
            SmallMessageObject o_returnSmallMessageObjectEmpty = (SmallMessageObject)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued small message object is null"));

            /* correct the small deviation with string null and empty string - empty strings are always interpreted as null */
            (o_returnMessageObject.StringArray ?? [])[5] = "";
            o_returnMessageObject.StringList[5] = "";

            o_messageObject.InitAll();

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_messageObject, o_returnMessageObject, p_b_useProperties, true, false),
                Is.True,
                "message object could not be retrieved identically"
            );

            o_messageObject.EmptyAll();

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_messageObject, o_returnMessageObjectEmpty, p_b_useProperties, true, true),
                Is.True,
                "empty message object could not be retrieved identically"
            );

            o_smallMessageObject.InitAll();

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_smallMessageObject, o_returnSmallMessageObject, p_b_useProperties, true, false),
                Is.True,
                "small message object could not be retrieved identically"
            );

            o_smallMessageObject.EmptyAll();

            Assert.That(
                ForestNET.Lib.Helper.ObjectsEqualUsingReflections(o_smallMessageObject, o_returnSmallMessageObjectEmpty, p_b_useProperties, true, true),
                Is.True,
                "empty small message object could not be retrieved identically"
            );
        }

        private static void TestMessageBoxMarshallingPrimitives(ForestNET.Lib.Net.Msg.MessageBox p_o_messageBox, int p_i_dataLengthInBytes, bool p_b_useProperties)
        {
            bool b_varBool = true;
            byte by_varByte = (byte)42;
            sbyte by_varSignedByte = (sbyte)-42;
            char c_varChar = (char)242;
            float f_varFloat = 42.25f;
            double d_varDouble = 42.75d;
            short sh_varShort = (short)-16426;
            ushort sh_varUnsignedShort = (ushort)16426;
            int i_varInteger = -536870954;
            uint i_varUnsignedInteger = (uint)536870954;
            long l_varLong = -1170936177994235946L;
            ulong l_varUnsignedLong = (ulong)1170936177994235946L;
            string? s_varString = "Hello World!";
            DateTime o_varTime = new(1970, 1, 1, 6, 2, 3);
            DateTime? o_varDate = new(2020, 3, 4);
            DateTime o_varDateTime = new(2020, 3, 4, 6, 2, 3);
            DateTime o_varLocalTime = new(1970, 1, 1, 6, 2, 3);
            DateTime o_varLocalDate = new(2020, 3, 4);
            DateTime? o_varLocalDateTime = new(2020, 3, 4, 6, 2, 3);
            decimal dec_varDecimal = -268435477.6710886925m;

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(b_varBool, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(by_varByte, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(by_varSignedByte, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(c_varChar, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(f_varFloat, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(d_varDouble, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(sh_varShort, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(sh_varUnsignedShort, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(i_varInteger, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(i_varUnsignedInteger, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(l_varLong, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(l_varUnsignedLong, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(s_varString, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varTime, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varDate, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varDateTime, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varLocalTime, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varLocalDate, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(o_varLocalDateTime, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(dec_varDecimal, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            bool b_varBoolDequeued = (bool)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            byte by_varByteDequeued = (byte)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            sbyte by_varSignedByteDequeued = (sbyte)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            char c_varCharDequeued = (char)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            float f_varFloatDequeued = (float)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            double d_varDoubleDequeued = (double)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            short sh_varShortDequeued = (short)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            ushort sh_varUnsignedShortDequeued = (ushort)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            int i_varIntegerDequeued = (int)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            uint i_varUnsignedIntegerDequeued = (uint)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            long l_varLongDequeued = (long)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            ulong l_varUnsignedLongDequeued = (ulong)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            string? s_varStringDequeued = (string?)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime o_varTimeDequeued = (DateTime)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime? o_varDateDequeued = (DateTime?)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime o_varDateTimeDequeued = (DateTime)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime o_varLocalTimeDequeued = (DateTime)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime o_varLocalDateDequeued = (DateTime)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime? o_varLocalDateTimeDequeued = (DateTime?)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            decimal dec_varDecimalDequeued = (decimal)(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));

            Assert.That(b_varBool, Is.EqualTo(b_varBoolDequeued), "message box transfer for 'boolean' primitive variable failed");
            Assert.That(by_varByte, Is.EqualTo(by_varByteDequeued), "message box transfer for 'byte' primitive variable failed");
            Assert.That(by_varSignedByte, Is.EqualTo(by_varSignedByteDequeued), "message box transfer for 'signed byte' primitive variable failed");
            Assert.That(c_varChar, Is.EqualTo(c_varCharDequeued), "message box transfer for 'char' primitive variable failed");
            Assert.That(f_varFloat, Is.EqualTo(f_varFloatDequeued), "message box transfer for 'float' primitive variable failed");
            Assert.That(d_varDouble, Is.EqualTo(d_varDoubleDequeued), "message box transfer for 'double' primitive variable failed");
            Assert.That(sh_varShort, Is.EqualTo(sh_varShortDequeued), "message box transfer for 'short' primitive variable failed");
            Assert.That(sh_varUnsignedShort, Is.EqualTo(sh_varUnsignedShortDequeued), "message box transfer for 'unsigned short' primitive variable failed");
            Assert.That(i_varInteger, Is.EqualTo(i_varIntegerDequeued), "message box transfer for 'int' primitive variable failed");
            Assert.That(i_varUnsignedInteger, Is.EqualTo(i_varUnsignedIntegerDequeued), "message box transfer for 'unsigned int' primitive variable failed");
            Assert.That(l_varLong, Is.EqualTo(l_varLongDequeued), "message box transfer for 'long' primitive variable failed");
            Assert.That(l_varUnsignedLong, Is.EqualTo(l_varUnsignedLongDequeued), "message box transfer for 'unsigned long' primitive variable failed");
            Assert.That(s_varString, Is.EqualTo(s_varStringDequeued), "message box transfer for 'string' primitive variable failed");
            Assert.That(o_varTime, Is.EqualTo(o_varTimeDequeued), "message box transfer for 'time' primitive variable failed");
            Assert.That(o_varDate, Is.EqualTo(o_varDateDequeued), "message box transfer for 'date' primitive variable failed");
            Assert.That(o_varDateTime, Is.EqualTo(o_varDateTimeDequeued), "message box transfer for 'date time' primitive variable failed");
            Assert.That(o_varLocalTime, Is.EqualTo(o_varLocalTimeDequeued), "message box transfer for 'local time' primitive variable failed");
            Assert.That(o_varLocalDate, Is.EqualTo(o_varLocalDateDequeued), "message box transfer for 'local date' primitive variable failed");
            Assert.That(o_varLocalDateTime, Is.EqualTo(o_varLocalDateTimeDequeued), "message box transfer for 'local date time' primitive variable failed");
            Assert.That(dec_varDecimal, Is.EqualTo(dec_varDecimalDequeued), "message box transfer for 'decimal' primitive variable failed");
        }

        private static void TestMessageBoxMarshallingArrays(ForestNET.Lib.Net.Msg.MessageBox p_o_messageBox, int p_i_dataLengthInBytes, bool p_b_useProperties)
        {
            bool[] a_varBoolArray = new bool[] { true, false, true, false, true };
            byte[] a_varByteArray = new byte[] { 1, 3, 5, (byte)133, 42, 0, (byte)102 };
            sbyte[] a_varSignedByteArray = new sbyte[] { 1, 3, 5, (sbyte)127, 42, 0, (sbyte)-102 };
            char[] a_varCharArray = new char[] { (char)65, (char)70, (char)75, (char)133, (char)85, (char)0, (char)243 };
            float[] a_varFloatArray = new float[] { 1.25f, 3.5f, 5.75f, 10.1010f, 41.998f, 0.0f, 4984654.5498795465f };
            double[] a_varDoubleArray = new double[] { 1.25d, 3.5d, 5.75d, 10.1010d, 41.998d, 0.0d, 8798546.2154656d };
            short[] a_varShortArray = new short[] { 1, 3, 5, -16426, 42, 0 };
            ushort[] a_varUnsignedShortArray = new ushort[] { 1, 3, 5, 16426, 42, 0 };
            int[] a_varIntegerArray = new int[] { 1, 3, 5, -536870954, 42, 0 };
            uint[] a_varUnsignedIntegerArray = new uint[] { 1, 3, 5, 536870954, 42, 0 };
            long[] a_varLongArray = new long[] { 1L, 3L, 5L, -1170936177994235946L, 42L, 0L };
            ulong[] a_varUnsignedLongArray = new ulong[] { 1L, 3L, 5L, 1170936177994235946L, 42L, 0L };
            string?[] a_varStringArray = new string?[] { "Hello World 1!", "Hello World 2!", "Hello World 3!", "Hello World 4!", "Hello World 5!", "", null };
            DateTime?[] a_varTimeArray = new DateTime?[] { new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null };
            DateTime?[] a_varDateArray = new DateTime?[] { new(2020, 3, 4), new(2020, 6, 8), new(2020, 12, 16), null };
            DateTime?[] a_varDateTimeArray = new DateTime?[] { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null };
            DateTime?[] a_varLocalTimeArray = new DateTime?[] { new(1970, 1, 1, 6, 2, 3), new(1970, 1, 1, 9, 24, 16), new(1970, 1, 1, 12, 48, 53), null };
            DateTime?[] a_varLocalDateArray = new DateTime?[] { new(2020, 3, 4), new(2020, 6, 8), new(2020, 12, 16), null };
            DateTime?[] a_varLocalDateTimeArray = new DateTime?[] { new(2020, 3, 4, 6, 2, 3), new(2020, 6, 8, 9, 24, 16), new(2020, 12, 16, 12, 48, 53), null };
            decimal[] a_varDecimalArray = new decimal[] { +578875020153.73804901109397m, -36.151686185423327m, +71740124.12171120119m, -2043204985254.1196m, 0m, +601.9924m };

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varBoolArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varByteArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varSignedByteArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varCharArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varFloatArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varDoubleArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varShortArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varUnsignedShortArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varIntegerArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varUnsignedIntegerArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varLongArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varUnsignedLongArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varStringArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varTimeArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varDateArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varDateTimeArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varLocalTimeArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varLocalDateArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varLocalDateTimeArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            if (!p_o_messageBox.EnqueueObjectWithMarshalling(a_varDecimalArray, p_i_dataLengthInBytes, p_b_useProperties))
            {
                throw new Exception("Could not enqueue object");
            }

            bool[] a_varBoolArrayDequeued = (bool[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            byte[] a_varByteArrayDequeued = (byte[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            sbyte[] a_varSignedByteArrayDequeued = (sbyte[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            char[] a_varCharArrayDequeued = (char[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            float[] a_varFloatArrayDequeued = (float[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            double[] a_varDoubleArrayDequeued = (double[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            short[] a_varShortArrayDequeued = (short[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            ushort[] a_varUnsignedShortArrayDequeued = (ushort[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            int[] a_varIntegerArrayDequeued = (int[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            uint[] a_varUnsignedIntegerArrayDequeued = (uint[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            long[] a_varLongArrayDequeued = (long[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            ulong[] a_varUnsignedLongArrayDequeued = (ulong[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            string?[] a_varStringArrayDequeued = (string?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varTimeArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varDateArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varDateTimeArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varLocalTimeArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varLocalDateArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            DateTime?[] a_varLocalDateTimeArrayDequeued = (DateTime?[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));
            decimal[] a_varDecimalArrayDequeued = (decimal[])(p_o_messageBox.DequeueObjectWithMarshalling(p_b_useProperties) ?? throw new NullReferenceException("dequeued message object is null"));

            /* correct the small deviation with string null and empty string - empty strings are always interpreted as null */
            a_varStringArrayDequeued[5] = "";

            Assert.That(a_varBoolArrayDequeued, Is.EqualTo(a_varBoolArray).AsCollection, "message box transfer for 'boolean' array variable failed");
            Assert.That(a_varByteArray, Is.EqualTo(a_varByteArrayDequeued).AsCollection, "message box transfer for 'byte' array variable failed");
            Assert.That(a_varSignedByteArray, Is.EqualTo(a_varSignedByteArrayDequeued).AsCollection, "message box transfer for 'signed byte' array variable failed");
            Assert.That(a_varCharArray, Is.EqualTo(a_varCharArrayDequeued).AsCollection, "message box transfer for 'char' array variable failed");
            Assert.That(a_varFloatArray, Is.EqualTo(a_varFloatArrayDequeued).AsCollection, "message box transfer for 'float' array variable failed");
            Assert.That(a_varDoubleArray, Is.EqualTo(a_varDoubleArrayDequeued).AsCollection, "message box transfer for 'double' array variable failed");
            Assert.That(a_varShortArray, Is.EqualTo(a_varShortArrayDequeued).AsCollection, "message box transfer for 'short' v variable failed");
            Assert.That(a_varUnsignedShortArray, Is.EqualTo(a_varUnsignedShortArrayDequeued).AsCollection, "message box transfer for 'unsigned short' array variable failed");
            Assert.That(a_varIntegerArray, Is.EqualTo(a_varIntegerArrayDequeued).AsCollection, "message box transfer for 'int' array variable failed");
            Assert.That(a_varUnsignedIntegerArray, Is.EqualTo(a_varUnsignedIntegerArrayDequeued).AsCollection, "message box transfer for 'unsigned int' array variable failed");
            Assert.That(a_varLongArray, Is.EqualTo(a_varLongArrayDequeued).AsCollection, "message box transfer for 'long' array variable failed");
            Assert.That(a_varUnsignedLongArray, Is.EqualTo(a_varUnsignedLongArrayDequeued).AsCollection, "message box transfer for 'unsigned long' array variable failed");
            Assert.That(a_varStringArray, Is.EqualTo(a_varStringArrayDequeued).AsCollection, "message box transfer for 'string' array variable failed");
            Assert.That(a_varTimeArray, Is.EqualTo(a_varTimeArrayDequeued).AsCollection, "message box transfer for 'time' array variable failed");
            Assert.That(a_varDateArray, Is.EqualTo(a_varDateArrayDequeued).AsCollection, "message box transfer for 'date' array variable failed");
            Assert.That(a_varDateTimeArray, Is.EqualTo(a_varDateTimeArrayDequeued).AsCollection, "message box transfer for 'date time' array variable failed");
            Assert.That(a_varLocalTimeArray, Is.EqualTo(a_varLocalTimeArrayDequeued).AsCollection, "message box transfer for 'local time' array variable failed");
            Assert.That(a_varLocalDateArray, Is.EqualTo(a_varLocalDateArrayDequeued).AsCollection, "message box transfer for 'local date' array variable failed");
            Assert.That(a_varLocalDateTimeArray, Is.EqualTo(a_varLocalDateTimeArrayDequeued).AsCollection, "message box transfer for 'local date time' array variable failed");
            Assert.That(a_varDecimalArray, Is.EqualTo(a_varDecimalArrayDequeued).AsCollection, "message box transfer for 'decimal' array variable failed");
        }
    }
}
