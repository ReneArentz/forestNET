namespace ForestNET.Tests.Net.Msg
{
    public class MessagBoxUnitTest
    {
        [Test]
        public void TestMessageBox()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                List<ForestNET.Tests.IO.Data.SimpleClass>? a_data =
                [
                    new("Record #1 Value A", "Record #1 Value B", "Record #1 Value C"),
                    new("Record #2 Value A", "Record #2 Value B", "", [1, 2, -3, -4]),
                    new("Record #3; Value A", "null", "Record #3 Value C", [9, 8, -7, -6], new float[] { 42.0f, 21.25f, 54987.456999f }),
                    new("Record 4 Value A", "Record $4 ;Value B \"", null, [16, 32, int.MaxValue, 128, 0], new float[] { 21.0f, 10.625f })
                ];

                ForestNET.Tests.IO.Data.SimpleClassCollection? o_foo = new(a_data);

                /* insert into message box */
                ForestNET.Lib.Net.Msg.MessageBox o_messageBox = new(1, 1500);

                if (!o_messageBox.EnqueueObject(o_foo))
                {
                    throw new Exception("Could not enqueue object");
                }

                /* clear variables */
                o_foo = null;
                a_data = null;

                /* remove from message box */
                o_foo = (ForestNET.Tests.IO.Data.SimpleClassCollection?)o_messageBox.DequeueObject();

                if (o_foo == null)
                {
                    throw new Exception("could not dequeue object, is null");
                }

                a_data = o_foo.SimpleClasses;
                int i = 0;

                foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_data)
                {
                    if (i == 0)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #1 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record #1 Value B"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo("Record #1 Value C"), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>()), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.Null, "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 1)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #2 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record #2 Value B"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo(""), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 1, 2, -3, -4 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.Null, "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #3; Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("null"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo("Record #3 Value C"), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 9, 8, -7, -6 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.EqualTo(new float[] { 42.0f, 21.25f, 54987.456999f }), "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record 4 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record $4 ;Value B \""), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.Null, "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 16, 32, int.MaxValue, 128, 0 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.EqualTo(new float[] { 21.0f, 10.625f }), "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }

                    i++;
                }

                /* insert into message box */
                if (!o_messageBox.EnqueueObject(a_data))
                {
                    throw new Exception("Could not enqueue object");
                }

                a_data = null;

                /* remove from message box */
                List<ForestNET.Tests.IO.Data.SimpleClass> a_foo = (List<ForestNET.Tests.IO.Data.SimpleClass>)(o_messageBox.DequeueObject() ?? Array.Empty<ForestNET.Tests.IO.Data.SimpleClass>().ToList());
                a_data = a_foo;

                if (a_data == null)
                {
                    throw new Exception("could not dequeue object, is null");
                }

                i = 0;

                foreach (ForestNET.Tests.IO.Data.SimpleClass o_simpleClassObject in a_data)
                {
                    if (i == 0)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #1 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record #1 Value B"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo("Record #1 Value C"), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>()), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.Null, "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 1)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #2 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record #2 Value B"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo(""), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 1, 2, -3, -4 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.Null, "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 2)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record #3; Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("null"), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.EqualTo("Record #3 Value C"), "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 9, 8, -7, -6 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.EqualTo(new float[] { 42.0f, 21.25f, 54987.456999f }), "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }
                    else if (i == 3)
                    {
                        Assert.That(o_simpleClassObject.ValueA, Is.EqualTo("Record 4 Value A"), "dequeued object #" + (i + 1) + " ValueA does not match with origin");
                        Assert.That(o_simpleClassObject.ValueB, Is.EqualTo("Record $4 ;Value B \""), "dequeued object #" + (i + 1) + " ValueB does not match with origin");
                        Assert.That(o_simpleClassObject.ValueC, Is.Null, "dequeued object #" + (i + 1) + " ValueC does not match with origin");
                        Assert.That(o_simpleClassObject.ValueD, Is.EqualTo(new List<int>() { 16, 32, int.MaxValue, 128, 0 }), "dequeued object #" + (i + 1) + " ValueD does not match with origin");
                        Assert.That(o_simpleClassObject.ValueE, Is.EqualTo(new float[] { 21.0f, 10.625f }), "dequeued object #" + (i + 1) + " ValueE does not match with origin");
                    }

                    i++;
                }
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
