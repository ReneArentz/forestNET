namespace Sandbox.Tests
{
    public class ConsoleProgressBarTest
    {
        public static void TestConsoleProgressBar()
        {
            WorkSimulationWithDelegate o_workSim;
            ForestNETLib.Core.ConsoleProgressBar o_progressBar_1 = new();

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_1.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_1.Init("Start progress () . . .", "Done ().");

            System.Threading.Thread o_thread = new(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_1.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            ForestNETLib.Core.ConsoleProgressBar o_progressBar_2 = new(18);

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_2.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_2.Init("Start progress (18) . . .", "Done (18).");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_2.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            ForestNETLib.Core.ConsoleProgressBar o_progressBar_3 = new((long)75, 18);

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_3.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_3.Init("Start progress (75, 18) . . .", "Done (75, 18).");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_3.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            ForestNETLib.Core.ConsoleProgressBar o_progressBar_4 = new((long)75, 18, 10);

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_4.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_4.Init("Start progress (75, 18, 10) . . .", "Done (75, 18, 10).", "Marquee test with large 38 text length");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_4.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            ForestNETLib.Core.ConsoleProgressBar o_progressBar_5 = new(10, 8);

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_5.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_5.Init("Start progress (10, 8) . . .", "Done (10, 8).", "Marquee test with large 38 text length");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_5.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            ForestNETLib.Core.ConsoleProgressBar o_progressBar_6 = new((long)75, 18, 10, 12);

            o_workSim = new WorkSimulationWithDelegate(
                p_i_progress =>
                {
                    o_progressBar_6.Report = (double)p_i_progress / 100.0d;
                }
            );

            o_progressBar_6.Init("Start progress (75, 18, 10, 12) . . .", "Done (75, 18, 10, 12).", "Marquee test with large 38 text length");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_6.Close();

            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */
            System.Console.WriteLine("\n" + "++++++++++++++++++++++++++++++++++++++++++" + "\n");
            /* +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ */

            o_progressBar_6.Init("Start progress (75, 18, 10, 12) . . .", "Done (75, 18, 10, 12).", "Short Marquee");

            o_thread = new System.Threading.Thread(o_workSim.Run);
            o_thread.Start();
            o_thread.Join();

            o_progressBar_6.Close();
        }
    }
}
