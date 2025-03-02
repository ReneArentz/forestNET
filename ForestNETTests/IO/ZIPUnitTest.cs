namespace ForestNETTests.IO
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
                string s_zipDirectory = s_currentDirectory + ForestNETLib.IO.File.DIR + "testZIP" + ForestNETLib.IO.File.DIR;

                if (ForestNETLib.IO.File.FolderExists(s_zipDirectory))
                {
                    ForestNETLib.IO.File.DeleteDirectory(s_zipDirectory);
                }

                ForestNETLib.IO.File.CreateDirectory(s_zipDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_zipDirectory),
                    Is.True,
                    "directory[" + s_zipDirectory + "] does not exist"
                );

                ForestNETLib.IO.File o_file = new(s_zipDirectory + "fileZip1.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                ForestNETLib.IO.File.ReplaceFileContent(s_zipDirectory + "fileZip1.txt", ForestNETLib.IO.File.GenerateRandomFileContent_50MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_zipDirectory + "fileZip1.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                ForestNETLib.IO.ZIP.Zip(s_zipDirectory + "fileZip1.txt", s_zipDirectory + "fileZip1.zip");

                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "fileZip1.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "fileZip1.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNETLib.IO.ZIP.CheckArchive(s_zipDirectory + "fileZip1.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "fileZip1.zip" + "] is not valid"
                );

                ForestNETLib.IO.File.DeleteFile(s_zipDirectory + "fileZip1.zip");
                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "fileZip1.zip"),
                    Is.False,
                    "file[" + s_zipDirectory + "fileZip1.zip" + "] does exist"
                );

                string s_subDirectory = s_zipDirectory + "sub" + ForestNETLib.IO.File.DIR;

                ForestNETLib.IO.File.CreateDirectory(s_subDirectory);

                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_subDirectory),
                    Is.True,
                    "directory[" + s_subDirectory + "] does not exist"
                );
                o_file = new ForestNETLib.IO.File(s_subDirectory + "fileZip1.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                o_file = new ForestNETLib.IO.File(s_subDirectory + "fileZip2.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                o_file = new ForestNETLib.IO.File(s_subDirectory + "fileZip3.txt", true);

                Assert.That(
                    o_file.FileContent, Is.Empty,
                    "file length != 0"
                );
                ForestNETLib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip1.txt", ForestNETLib.IO.File.GenerateRandomFileContent_1MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_subDirectory + "fileZip1.txt"), Is.EqualTo(1048576),
                    "file length != 1048576"
                );
                ForestNETLib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip2.txt", ForestNETLib.IO.File.GenerateRandomFileContent_50MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_subDirectory + "fileZip2.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                ForestNETLib.IO.File.ReplaceFileContent(s_subDirectory + "fileZip3.txt", ForestNETLib.IO.File.GenerateRandomFileContent_10MB(), System.Text.Encoding.ASCII);
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_subDirectory + "fileZip3.txt"), Is.EqualTo(10485760),
                    "file length != 10485760"
                );
                ForestNETLib.IO.ZIP.Zip(s_subDirectory, s_zipDirectory + "folder.zip");

                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "folder.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "folder.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNETLib.IO.ZIP.CheckArchive(s_zipDirectory + "folder.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "folder.zip" + "] is not valid"
                );
                Assert.That(
                    ForestNETLib.IO.ZIP.GetSize(s_zipDirectory + "folder.zip"), Is.EqualTo(63963136),
                    "zip size != 63963136"
                );
                ForestNETLib.IO.File.DeleteFile(s_zipDirectory + "folder.zip");
                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "folder.zip"),
                    Is.False,
                    "file[" + s_zipDirectory + "folder.zip" + "] does exist"
                );

                ForestNETLib.IO.ZIP.Zip(s_zipDirectory, s_zipDirectory + "all.zip");

                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "all.zip"),
                    Is.True,
                    "file[" + s_zipDirectory + "all.zip" + "] does not exist"
                );
                Assert.That(
                    ForestNETLib.IO.ZIP.CheckArchive(s_zipDirectory + "all.zip"),
                    Is.True,
                    "archive[" + s_zipDirectory + "all.zip" + "] is not valid"
                );
                Assert.That(
                    ForestNETLib.IO.ZIP.GetSize(s_zipDirectory + "all.zip"), Is.EqualTo(116391936),
                    "zip size != 116391936"
                );

                ForestNETLib.IO.File.DeleteDirectory(s_subDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_subDirectory),
                    Is.False,
                    "directory[" + s_subDirectory + "] does exist"
                );
                ForestNETLib.IO.File.DeleteFile(s_zipDirectory + "fileZip1.txt");
                Assert.That(
                    ForestNETLib.IO.File.Exists(s_zipDirectory + "fileZip1.txt"),
                    Is.False,
                    "file[" + s_zipDirectory + "fileZip1.txt" + "] does exist"
                );

                string s_unzipDirectory = s_zipDirectory + "unzip" + ForestNETLib.IO.File.DIR;

                ForestNETLib.IO.ZIP.Unzip(s_zipDirectory + "all.zip", s_unzipDirectory, true, true);

                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_unzipDirectory + "fileZip1.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNETLib.IO.File.DIR + "fileZip1.txt"), Is.EqualTo(1048576),
                    "file length != 1048576"
                );
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNETLib.IO.File.DIR + "fileZip2.txt"), Is.EqualTo(52428800),
                    "file length != 52428800"
                );
                Assert.That(
                    ForestNETLib.IO.File.FileLength(s_unzipDirectory + "sub" + ForestNETLib.IO.File.DIR + "fileZip3.txt"), Is.EqualTo(10485760),
                    "file length != 10485760"
                );

                ForestNETLib.IO.File.DeleteDirectory(s_zipDirectory);
                Assert.That(
                    ForestNETLib.IO.File.FolderExists(s_zipDirectory),
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
