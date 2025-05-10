namespace ForestNET.Tests.Net.FTP
{
    public class ClientUnitTest
    {
        /**
		 * !!!!!!!!!!!!!!!!!
		 * it is maybe necessary to create new certificate file, or rather update 'proftpd.crt' in test resources
		 * !!!!!!!!!!!!!!!!!
		 */
        private static readonly string PathToFTPSCertificate = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ">> cannot find current directory <<")
            + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ftps" + ForestNET.Lib.IO.File.DIR
            + "proftpd.crt";

        [Test]
        public void TestClient()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                RunFtp("172.24.87.100", 12220, "user", "user", "/");
                RunFtps("172.24.87.100", 12221, "user", "user", "/");
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void RunFtp(string p_s_ftpHostIp, int p_i_ftpPort, string p_s_ftpUser, string p_s_ftpPassword, string p_s_startingFolderRemote)
        {
            string s_ftpUrlPrefix = "ftp://";
            string s_ftpUrl;

            s_ftpUrl = s_ftpUrlPrefix + p_s_ftpHostIp;
            ForestNET.Lib.Net.FTP.Client o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            o_ftpClient = new(s_ftpUrl, p_i_ftpPort, p_s_ftpUser, p_s_ftpPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);

            s_ftpUrl = s_ftpUrlPrefix + p_s_ftpUser + ":" + p_s_ftpPassword + "@" + p_s_ftpHostIp + ":" + p_i_ftpPort;
            o_ftpClient = new(s_ftpUrl)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false
            };
            RunRoutine(o_ftpClient, p_s_startingFolderRemote, o_ftpClient.Encoding);
        }

        private static void RunFtps(string p_s_ftpsHostIp, int p_i_ftpsPort, string p_s_ftpsUser, string p_s_ftpsPassword, string p_s_startingFolderRemote)
        {
            string s_ftpsUrlPrefix = "ftps://";
            string s_ftpsUrl;

            s_ftpsUrl = s_ftpsUrlPrefix + p_s_ftpsHostIp;
            ForestNET.Lib.Net.FTP.Client o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = true,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            o_ftpsClient = new(s_ftpsUrl, p_i_ftpsPort, p_s_ftpsUser, p_s_ftpsPassword)
            {
                PreferMLSD = false,
                Encoding = System.Text.Encoding.ASCII,
                UsePassiveMode = false,
                UseBinary = false,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);

            s_ftpsUrl = s_ftpsUrlPrefix + p_s_ftpsUser + ":" + p_s_ftpsPassword + "@" + p_s_ftpsHostIp + ":" + p_i_ftpsPort;
            o_ftpsClient = new(s_ftpsUrl)
            {
                PreferMLSD = true,
                Encoding = System.Text.Encoding.UTF8,
                UsePassiveMode = true,
                UseBinary = true,
                KeepAlive = false,
                UseExplicitEncryptionMode = true
            };
            o_ftpsClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(PathToFTPSCertificate));
            RunRoutine(o_ftpsClient, p_s_startingFolderRemote, o_ftpsClient.Encoding);
        }

        private static void RunRoutine(ForestNET.Lib.Net.FTP.Client? p_o_ftpClient, string p_s_startingFolderRemote, System.Text.Encoding p_o_encoding)
        {
            string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
            string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFtp" + ForestNET.Lib.IO.File.DIR;
            string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ftp" + ForestNET.Lib.IO.File.DIR;

            if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
            {
                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
            }

            ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                Is.True,
                "directory[" + s_testDirectory + "] does not exist"
            );

            Assert.That(p_o_ftpClient, Is.Not.Null, "ftp(s) client object is null");

            Assert.That(p_o_ftpClient.Login().Result, Is.True, "could not login; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("257"), "ftp reply code is not '257', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.MkDir(p_s_startingFolderRemote + "second_folder").Result, Is.True, "folder '" + p_s_startingFolderRemote + "second_folder' could not be created; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("200"), "ftp reply code is not '200', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Upload(p_o_encoding.GetBytes("Hello World!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", false).Result, Is.True, "could not upload bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("150"), "ftp reply code is not '150', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Upload(p_o_encoding.GetBytes("Hello World!!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", true).Result, Is.True, "could not append bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("150"), "ftp reply code is not '150', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Download(p_s_startingFolderRemote + "first_subfolder/test.txt", s_testDirectory + "text.txt").Result, Is.True, "could not download file from '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("150"), "ftp reply code is not '150', but '" + p_o_ftpClient.FtpReplyCode + "'");

            ForestNET.Lib.IO.File o_file = new(s_testDirectory + "text.txt", false);
            Assert.That(o_file.FileContent, Is.EqualTo("Hello World!\r\nHello World!!\r\n"), "downloaded file content from '" + p_s_startingFolderRemote + "first_subfolder/test.txt' is not equal to uploaded bytes from before");

            Assert.That(p_o_ftpClient.Rename(p_s_startingFolderRemote + "first_subfolder/test.txt", p_s_startingFolderRemote + "renamed_test.txt").Result, Is.True, "could not rename file '" + p_s_startingFolderRemote + "first_subfolder/test.txt' to '" + p_s_startingFolderRemote + "renamed_test.txt'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("250"), "ftp reply code is not '250', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Rename(p_s_startingFolderRemote + "first_subfolder", p_s_startingFolderRemote + "delete_subfolder").Result, Is.True, "could not rename folder '" + p_s_startingFolderRemote + "first_subfolder' to '" + p_s_startingFolderRemote + "delete_subfolder'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("250"), "ftp reply code is not '250', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder").Result, Is.True, "directory '" + p_s_startingFolderRemote + "delete_subfolder' does not exist; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("226"), "ftp reply code is not '226', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt").Result, Is.True, "file '" + p_s_startingFolderRemote + "renamed_test.txt' does not exist; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("226"), "ftp reply code is not '226', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Upload(s_resourcesDirectory + "1-MB-Test.xlsx", p_s_startingFolderRemote + "second_folder/singleFile.xlsx", false).Result, Is.True, "could not upload file '1-MB-Test.xlsx' to '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("150"), "ftp reply code is not '150', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.GetLength(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result, Is.EqualTo(
                    Convert.ToInt64(
                        (p_o_ftpClient.Download(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result ?? []).Length
                    )
                ), "file length from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx' is not equal do downloaded bytes from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("150"), "ftp reply code is not '150', but '" + p_o_ftpClient.FtpReplyCode + "'");

            p_o_ftpClient.UploadFolder(s_resourcesDirectory, p_s_startingFolderRemote + "delete_subfolder/test", true).Wait();

            int i_countFiles = 0;
            int i_coundDirectories = 0;

            foreach (ForestNET.Lib.Net.FTP.Entry o_entry in p_o_ftpClient.Ls(p_s_startingFolderRemote + "delete_subfolder/test", false, false, p_o_ftpClient.PreferMLSD, true).Result)
            {
                if (o_entry.IsDirectory)
                {
                    i_coundDirectories++;
                }
                else
                {
                    i_countFiles++;
                }
            }

            Assert.That(i_coundDirectories, Is.EqualTo(1), "found remote directories under '" + p_s_startingFolderRemote + "delete_subfolder/test' are not '1', but '" + i_coundDirectories + "'");
            Assert.That(i_countFiles, Is.EqualTo(6), "found remote files under '" + p_s_startingFolderRemote + "delete_subfolder/test' are not '6', but '" + i_countFiles + "'");

            p_o_ftpClient.DownloadFolder(p_s_startingFolderRemote + "delete_subfolder", s_testDirectory + ForestNET.Lib.IO.File.DIR + "download_folder", true, true).Wait();

            List<ForestNET.Lib.IO.ListingElement> a_list = ForestNET.Lib.IO.File.ListDirectory(s_testDirectory + "download_folder" + ForestNET.Lib.IO.File.DIR + "test", true);

            i_countFiles = 0;
            i_coundDirectories = 0;

            foreach (ForestNET.Lib.IO.ListingElement o_entry in a_list)
            {
                if (o_entry.IsDirectory)
                {
                    i_coundDirectories++;
                }
                else
                {
                    i_countFiles++;
                }
            }

            Assert.That(i_coundDirectories, Is.EqualTo(1), "found local directories under './download_folder' are not '1', but '" + i_coundDirectories + "'");
            Assert.That(i_countFiles, Is.EqualTo(6), "found local files under './download_folder' are not '6', but '" + i_countFiles + "'");

            long l_left = ForestNET.Lib.IO.File.FileLength(s_resourcesDirectory + "10-MB-Test.xlsx");

            /* expect other file length if we not using binary mode */
            if (!p_o_ftpClient.UseBinary)
            {
                l_left = 9565856;
            }

            long l_right = ForestNET.Lib.IO.File.FileLength(s_testDirectory + "download_folder" + ForestNET.Lib.IO.File.DIR + "test" + ForestNET.Lib.IO.File.DIR + "10-MB-Test.xlsx");

            Assert.That(l_left, Is.EqualTo(l_right), "file length of '10-MB-Test.xlsx' in upload sources and downloaded folder are not equal: '" + l_left + "' != '" + l_right + "'");

            Assert.That(p_o_ftpClient.Delete(p_s_startingFolderRemote + "renamed_test.txt").Result, Is.True, "could not delete file '" + p_s_startingFolderRemote + "renamed_test.txt'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("250"), "ftp reply code is not '250', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.RmDir(p_s_startingFolderRemote + "delete_subfolder").Result, Is.True, "could not delete directory '" + p_s_startingFolderRemote + "delete_subfolder'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("200"), "ftp reply code is not '200', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.RmDir(p_s_startingFolderRemote + "second_folder").Result, Is.True, "could not delete directory '" + p_s_startingFolderRemote + "second_folder'; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("200"), "ftp reply code is not '200', but '" + p_o_ftpClient.FtpReplyCode + "'");

            Assert.That(p_o_ftpClient.Logout().Result, Is.True, "could not logout; " + p_o_ftpClient.FtpReply);
            Assert.That(p_o_ftpClient.FtpReplyCode, Is.EqualTo("221"), "ftp reply code is not '221' for logout, but '" + p_o_ftpClient.FtpReplyCode + "'");

            ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                Is.False,
                "directory[" + s_testDirectory + "] does exist"
            );
        }
    }
}
