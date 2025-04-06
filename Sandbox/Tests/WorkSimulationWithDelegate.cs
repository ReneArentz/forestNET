namespace Sandbox.Tests
{
    public class WorkSimulationWithDelegate : ForestNET.Lib.Runnable
    {

        /* Delegates */

        public delegate void PostProgress(int p_i_progress);

        /* Fields */

        private readonly PostProgress del_postProgress;

        /* Properties */

        /* Methods */

        public WorkSimulationWithDelegate(PostProgress p_del_postProgress)
        {
            this.del_postProgress = p_del_postProgress;
        }

        override public void Run()
        {
            for (int i = 0; i < 100; i++)
            {
                this.del_postProgress?.Invoke(i);

                int i_sleep = ForestNET.Lib.Helper.RandomIntegerRange(25, 100);

                try
                {
                    System.Threading.Thread.Sleep(i_sleep);
                }
                catch (Exception)
                {
                    /* nothing to do */
                }
            }

            this.del_postProgress?.Invoke(100);
        }
    }
}
