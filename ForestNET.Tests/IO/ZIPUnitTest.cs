namespace ForestNET.Tests.IO
{
    public class ZIPUnitTest
    {
        [Test]
        public void TestZIP()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_zipDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testZIP" + ForestNET.Lib.IO.File.DIR;

                if (ForestNET.Lib.IO.File.FolderExists(s_zipDirectory))
                {
                    ForestNET.Lib.IO.File.DeleteDirectory(s_zipDirectory);
                }

                ForestNET.Lib.IO.File.CreateDirectory(s_zipDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_zipDirectory),
                    Is.True,
                    "directory[" + s_zipDirectory + "] does not exist"
                );

                ForestNET.Lib.IO.File o_file = new(s_zipDirectory + "fileZip1.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                ForestNET.Lib.IO.File.ReplaceFileContent(s_zipDirectory + "fileZip1.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_50MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_zipDirectory + "fileZip1.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                ForestNET.Lib.IO.ZIP.Zip(s_zipDirectory + "fileZip1.txt", s_zipDirectory + "fileZip1.zip");

                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "fileZip1.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "fileZip1.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.ZIP.CheckArchive(s_zipDirectory + "fileZip1.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "fileZip1.zip" + "] is not valid"
                );

                ForestNET.Lib.IO.File.DeleteFile(s_zipDirectory + "fileZip1.zip");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "fileZip1.zip"),
                    Is.False,
                    "file[" + s_zipDirectory + "fileZip1.zip" + "] does exist"
                );

                string s_subDirectory = s_zipDirectory + "sub" + ForestNET.Lib.IO.File.DIR;

                ForestNET.Lib.IO.File.CreateDirectory(s_subDirectory);

                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_subDirectory),
                    Is.True,
                    "directory[" + s_subDirectory + "] does not exist"
                );
                o_file = new ForestNET.Lib.IO.File(s_subDirectory + "fileZip1.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                o_file = new ForestNET.Lib.IO.File(s_subDirectory + "fileZip2.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                o_file = new ForestNET.Lib.IO.File(s_subDirectory + "fileZip3.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                ForestNET.Lib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip1.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_1MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_subDirectory + "fileZip1.txt"), Is.EqualTo(1048576),
                    "file length != 1048576"
                );
                ForestNET.Lib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip2.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_50MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_subDirectory + "fileZip2.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                ForestNET.Lib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip3.txt", ForestNET.Lib.IO.File.GenerateRandomFileContent_10MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_subDirectory + "fileZip3.txt"), Is.EqualTo(10485760),
                    "file length != 10485760"
                );
                ForestNET.Lib.IO.ZIP.Zip(s_subDirectory, s_zipDirectory + "folder.zip");

                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "folder.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "folder.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.ZIP.CheckArchive(s_zipDirectory + "folder.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "folder.zip" + "] is not valid"
                );
                Assert.That(
                    ForestNET.Lib.IO.ZIP.GetSize(s_zipDirectory + "folder.zip"), Is.EqualTo(63963136),
                    "zip size != 63963136"
                );
                ForestNET.Lib.IO.File.DeleteFile(s_zipDirectory + "folder.zip");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "folder.zip"),
                    Is.False,
                    "file[" + s_zipDirectory + "folder.zip" + "] does exist"
                );

                ForestNET.Lib.IO.ZIP.Zip(s_zipDirectory, s_zipDirectory + "all.zip");

                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "all.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "all.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNET.Lib.IO.ZIP.CheckArchive(s_zipDirectory + "all.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "all.zip" + "] is not valid"
                );
                Assert.That(
                    ForestNET.Lib.IO.ZIP.GetSize(s_zipDirectory + "all.zip"), Is.EqualTo(116391936),
                    "zip size != 116391936"
                );

                ForestNET.Lib.IO.File.DeleteDirectory(s_subDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_subDirectory),
                    Is.False,
                    "directory[" + s_subDirectory + "] does exist"
                );
                ForestNET.Lib.IO.File.DeleteFile(s_zipDirectory + "fileZip1.txt");
                Assert.That(
                    ForestNET.Lib.IO.File.Exists(s_zipDirectory + "fileZip1.txt"),
                    Is.False,
                    "file[" + s_zipDirectory + "fileZip1.txt" + "] does exist"
                );

                string s_unzipDirectory = s_zipDirectory + "unzip" + ForestNET.Lib.IO.File.DIR;

                ForestNET.Lib.IO.ZIP.Unzip(s_zipDirectory + "all.zip", s_unzipDirectory, true, true);

                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_unzipDirectory + "fileZip1.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "fileZip1.txt"), Is.EqualTo(1048576),
                    "file length != 1048576"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "fileZip2.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                Assert.That(
                    ForestNET.Lib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNET.Lib.IO.File.DIR + "fileZip3.txt"), Is.EqualTo(10485760),
                    "file length != 10485760"
                );

                ForestNET.Lib.IO.File.DeleteDirectory(s_zipDirectory);
                Assert.That(
                    ForestNET.Lib.IO.File.FolderExists(s_zipDirectory),
                    Is.False,
                    "directory[" + s_zipDirectory + "] does exist"
                );
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }
    }
}
