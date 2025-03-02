namespace Sandbox.Tests
{
    public class ZipTest
    {
        public static void TestZipProgressBar()
        {
            string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNETLib.IO.File.DIR;
            string s_testDirectory = s_currentDirectory + "testZipProgressBar" + ForestNETLib.IO.File.DIR;

            if (ForestNETLib.IO.File.FolderExists(s_testDirectory))
            {
                ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
            }

            ForestNETLib.IO.File.CreateDirectory(s_testDirectory);

            string s_file = s_testDirectory + "sourceFile.txt";
            string s_zipFile = s_testDirectory + "zippedFile.zip";
            string s_originalFile = s_testDirectory + "originalSourceFile.txt";

            _ = new ForestNETLib.IO.File(s_file, true);

            ForestNETLib.IO.File.ReplaceFileContent(s_file, ForestNETLib.IO.File.GenerateRandomFileContent_250MB(), System.Text.Encoding.ASCII);

            if (ForestNETLib.IO.File.FileLength(s_file) != 250L * 1024L * 1024L)
            {
                throw new Exception("file length != " + 250L * 1024L * 1024L + " bytes; it is '" + ForestNETLib.IO.File.FileLength(s_file) + "'");
            }

            ForestNETLib.Core.ConsoleProgressBar o_consoleProgressBar = new();

            ForestNETLib.IO.ZIP.PostProgress del_postProgress = (double p_d_progress) =>
            {
                o_consoleProgressBar.Report = p_d_progress;
            };

            o_consoleProgressBar.Init();
            o_consoleProgressBar.Close();

            o_consoleProgressBar.Init("Zip . . .", "Done.");
            ForestNETLib.IO.ZIP.Zip(s_file, s_zipFile, System.IO.Compression.CompressionLevel.Optimal, del_postProgress);
            o_consoleProgressBar.Close();

            Console.WriteLine("Zipped '" + s_file + "' to '" + s_zipFile + "'");

            o_consoleProgressBar.Init("Check archive . . .", "Done.");
            bool b_valid = ForestNETLib.IO.ZIP.CheckArchive(s_zipFile, del_postProgress);
            o_consoleProgressBar.Close();

            if (b_valid)
            {
                Console.WriteLine("Zip file '" + s_zipFile + "' is valid");
            }
            else
            {
                Console.WriteLine("Zip file '" + s_zipFile + "' is corrupted");
            }

            ForestNETLib.IO.File.MoveFile(s_file, s_originalFile);

            o_consoleProgressBar.Init("Unzip . . .", "Done.");
            ForestNETLib.IO.ZIP.Unzip(s_zipFile, s_testDirectory, true, true, del_postProgress);
            o_consoleProgressBar.Close();

            Console.WriteLine("Unzipped '" + s_zipFile + "' to '" + s_file + "'");

            string s_hashSource = ForestNETLib.IO.File.HashFile(s_file, "SHA-256") ?? throw new NullReferenceException("return of HashFile is null");
            string s_hashDestination = ForestNETLib.IO.File.HashFile(s_originalFile, "SHA-256") ?? throw new NullReferenceException("return of HashFile is null");

            if (!s_hashSource.Equals(s_hashDestination))
            {
                throw new Exception("hash value of source and destination file are not matching: '" + s_hashSource + "' != '" + s_hashDestination + "'");
            }
            else
            {
                Console.WriteLine("Unzipped file matches with original source");
            }

            ForestNETLib.IO.File.DeleteDirectory(s_testDirectory);
        }
    }
}
