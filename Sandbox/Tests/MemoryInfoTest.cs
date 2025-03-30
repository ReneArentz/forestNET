namespace Sandbox.Tests
{
    public class MemoryInfoTest
    {
        public static void TestMemoryInfo()
        {
            ForestNET.Lib.MemoryInfo o_memoryInfo = new(ForestNET.Lib.Global.Instance.LOG ?? throw new NullReferenceException("LOG is not set"), 1000, ForestNET.Lib.Log.Level.INFO);
            System.Threading.Thread o_memoryInfoThread = new(o_memoryInfo.Run);

            ForestNET.Lib.Global.Log("starting memory info thread . . .");

            o_memoryInfoThread.Start();

            ForestNET.Lib.Global.Log("calculate pi and occupy memory . . .");

            //System.Collections.Generic.List<string> a_foo = [];
            double d_pi = 0.0d;

            for (int i = 1_000_000_000; i > 0; i--)
            {
                d_pi += Math.Pow(-1, i + 1) / (2 * i - 1); /* calculate series in parenthesis */

                /* uncomment these lines to use more RAM */
                //if (i % 1_000 == 0)
                //{
                //	a_foo.Add(d_pi.ToString());
                //}

                if (i == 1)
                {
                    d_pi *= 4;
                }
            }

            ForestNET.Lib.Global.LogWarning(d_pi.ToString()); /* print pi */

            if (o_memoryInfo != null)
            {
                o_memoryInfo.Stop = true;
                System.Threading.Thread.Sleep(2000);
            }

            ForestNET.Lib.Global.Log("memory info thread stopped . . .");
        }
    }
}
