namespace Sandbox.Tests.Net.Request
{
    public class WebRequestTest
    {
        public static void TestWebRequest(string p_s_httpsUrl)
        {
            string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;
            string s_testDirectory = s_currentDirectory + "testWebRequestProgressBar" + ForestNET.Lib.IO.File.DIR;

            if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
            {
                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
            }

            ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);

            ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

            ForestNET.Lib.Net.Request.Client.PostProgress del_postProgress = (double p_d_progress, string p_s_filename) =>
            {
                o_consoleProgressBar.MarqueeText = p_s_filename;
                o_consoleProgressBar.Report = p_d_progress;
            };

            string s_file = s_testDirectory + "jdk.zip";

            ForestNET.Lib.Net.Request.Client o_webRequestDownload = new(ForestNET.Lib.Net.Request.RequestType.DOWNLOAD, p_s_httpsUrl)
            {
                DownloadFilename = s_file,
                DelegatePostProgress = del_postProgress
            };

            o_consoleProgressBar.Init("Download file . . .", "Download finished.");
            string s_response = o_webRequestDownload.ExecuteWebRequest().Result;
            o_consoleProgressBar.Close();

            Console.WriteLine(s_response);

            if (o_webRequestDownload.ResponseCode != 200)
            {
                throw new Exception("Response code is not '200', it is '" + o_webRequestDownload.ResponseCode + "'");
            }

            if (!o_webRequestDownload.ResponseMessage.Equals("OK"))
            {
                throw new Exception("Response code is not 'OK', it is '" + o_webRequestDownload.ResponseMessage + "'");
            }

            Console.WriteLine("file length = '" + ForestNET.Lib.IO.File.FileLength(s_file) + " bytes'");

            ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
        }
    }
}
