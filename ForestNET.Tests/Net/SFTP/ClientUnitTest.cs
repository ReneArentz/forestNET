namespace ForestNET.Tests.Net.SFTP
{
    public class ClientUnitTest
    {
        [Test]
        public void TestClient()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                /**
			     * !!!!!!!!!!!!!!!!!
			     * it is maybe necessary to create new authentication and known_hosts files
			     * known_hosts entry must be base64 string of SHA-256 fingerprint
			     * !!!!!!!!!!!!!!!!!
			     */

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "sftp" + ForestNET.Lib.IO.File.DIR;

                SftpTestCredentials o_sftpCredentials = new()
                {
                    HostIp = "172.24.87.100",
                    Port = 2222,
                    User = "user",
                    Password = "user",
                    FilePathAuthentication = s_resourcesDirectory + "sftp_id_rsa"
                };

                SftpTestCredentials o_sshCredentials = new()
                {
                    HostIp = "172.24.91.23",
                    LocalPort = 2223,
                    Port = 22,
                    User = "userssh",
                    Password = "userssh",
                    FilePathAuthentication = s_resourcesDirectory + "ssh_id_rsa"
                };

                /* test sftp */
                RunSftp(o_sftpCredentials, null, s_resourcesDirectory + "known_hosts", "/");
                /* test sftp with ssh tunnel */
                RunSftp(o_sftpCredentials, o_sshCredentials, s_resourcesDirectory + "known_hosts", "/");
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private class SftpTestCredentials
        {
            public string HostIp { get; set; } = string.Empty;
            public int LocalPort { get; set; } = 0; /* just for ssh tunnel */
            public int Port { get; set; } = 0;
            public string User { get; set; } = string.Empty;
            public string? Password { get; set; } = null;
            public string? FilePathAuthentication { get; set; } = null;
        }

        private static void RunSftp(SftpTestCredentials p_o_sftpCredentials, SftpTestCredentials? p_o_sshCredentials, string p_s_filePathKnownHosts, string p_s_startingFolderRemote)
        {
            string s_ftpUrlPrefix = "sftp://";
            string s_ftpUrl;

            if (p_o_sshCredentials == null)
            {
                s_ftpUrl = s_ftpUrlPrefix + p_o_sftpCredentials.HostIp;
                ForestNET.Lib.Net.SFTP.Client o_sftpClient = new(s_ftpUrl, p_o_sftpCredentials.Port, p_o_sftpCredentials.User, p_o_sftpCredentials.FilePathAuthentication, null, p_s_filePathKnownHosts)
                {
                    Encoding = System.Text.Encoding.UTF8,
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                o_sftpClient = new(s_ftpUrl, p_o_sftpCredentials.Port, p_o_sftpCredentials.User, p_o_sftpCredentials.FilePathAuthentication, null, p_s_filePathKnownHosts)
                {
                    Encoding = System.Text.Encoding.ASCII,
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                s_ftpUrl = s_ftpUrlPrefix + p_o_sftpCredentials.User + ":" + p_o_sftpCredentials.Password + "@" + p_o_sftpCredentials.HostIp + ":" + p_o_sftpCredentials.Port;
                o_sftpClient = new(s_ftpUrl)
                {
                    Encoding = System.Text.Encoding.UTF8,
                    FilePathKnownHosts = p_s_filePathKnownHosts
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                o_sftpClient = new(s_ftpUrl)
                {
                    Encoding = System.Text.Encoding.ASCII,
                    FilePathKnownHosts = p_s_filePathKnownHosts
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);
            }
            else
            {
                s_ftpUrl = s_ftpUrlPrefix + p_o_sftpCredentials.HostIp;
                ForestNET.Lib.Net.SFTP.Client o_sftpClient = new(s_ftpUrl, p_o_sftpCredentials.Port, p_o_sftpCredentials.User, p_o_sftpCredentials.FilePathAuthentication, null, p_s_filePathKnownHosts)
                {
                    Encoding = System.Text.Encoding.UTF8,
                    TunnelHost = p_o_sshCredentials.HostIp,
                    TunnelLocalPort = p_o_sshCredentials.LocalPort,
                    TunnelPort = p_o_sshCredentials.Port,
                    TunnelUser = p_o_sshCredentials.User,
                    TunnelPassword = null,
                    TunnelFilePathAuthentication = p_o_sshCredentials.FilePathAuthentication
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                o_sftpClient = new(s_ftpUrl, p_o_sftpCredentials.Port, p_o_sftpCredentials.User, p_o_sftpCredentials.FilePathAuthentication, null, p_s_filePathKnownHosts)
                {
                    Encoding = System.Text.Encoding.ASCII,
                    TunnelHost = p_o_sshCredentials.HostIp,
                    TunnelLocalPort = p_o_sshCredentials.LocalPort,
                    TunnelPort = p_o_sshCredentials.Port,
                    TunnelUser = p_o_sshCredentials.User,
                    TunnelPassword = p_o_sshCredentials.Password,
                    TunnelFilePathAuthentication = null
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                s_ftpUrl = s_ftpUrlPrefix + p_o_sftpCredentials.User + ":" + p_o_sftpCredentials.Password + "@" + p_o_sftpCredentials.HostIp + ":" + p_o_sftpCredentials.Port;
                o_sftpClient = new(s_ftpUrl)
                {
                    Encoding = System.Text.Encoding.UTF8,
                    FilePathKnownHosts = p_s_filePathKnownHosts,
                    TunnelHost = p_o_sshCredentials.HostIp,
                    TunnelLocalPort = p_o_sshCredentials.LocalPort,
                    TunnelPort = p_o_sshCredentials.Port,
                    TunnelUser = p_o_sshCredentials.User,
                    TunnelPassword = null,
                    TunnelFilePathAuthentication = p_o_sshCredentials.FilePathAuthentication
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);

                o_sftpClient = new(s_ftpUrl)
                {
                    Encoding = System.Text.Encoding.ASCII,
                    FilePathKnownHosts = p_s_filePathKnownHosts,
                    TunnelHost = p_o_sshCredentials.HostIp,
                    TunnelLocalPort = p_o_sshCredentials.LocalPort,
                    TunnelPort = p_o_sshCredentials.Port,
                    TunnelUser = p_o_sshCredentials.User,
                    TunnelPassword = p_o_sshCredentials.Password,
                    TunnelFilePathAuthentication = null
                };
                RunRoutine(o_sftpClient, p_s_startingFolderRemote, o_sftpClient.Encoding);
            }
        }

        private static void RunRoutine(ForestNET.Lib.Net.SFTP.Client? p_o_sftpClient, string p_s_startingFolderRemote, System.Text.Encoding p_o_encoding)
        {
            string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
            string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testSftp" + ForestNET.Lib.IO.File.DIR;
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

            Assert.That(p_o_sftpClient, Is.Not.Null, "sftp client object is null");

            Assert.That(p_o_sftpClient.Login().Result, Is.True, "could not login");

            p_o_sftpClient.MkDir(p_s_startingFolderRemote + "second_folder");
            Assert.That(p_o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "second_folder"), Is.True, "folder '" + p_s_startingFolderRemote + "second_folder' could not be created");

            p_o_sftpClient.Upload(p_o_encoding.GetBytes("Hello World!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", false).Wait();
            Assert.That(p_o_sftpClient.FileExists(p_s_startingFolderRemote + "first_subfolder/test.txt"), Is.True, "could not upload bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");

            p_o_sftpClient.Upload(p_o_encoding.GetBytes("Hello World!!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", true).Wait();
            Assert.That(p_o_sftpClient.FileExists(p_s_startingFolderRemote + "first_subfolder/test.txt"), Is.True, "could not append bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");

            p_o_sftpClient.Download(p_s_startingFolderRemote + "first_subfolder/test.txt", s_testDirectory + "text.txt").Wait();
            Assert.That(ForestNET.Lib.IO.File.Exists(s_testDirectory + "text.txt"), Is.True, "could not download file from '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");

            ForestNET.Lib.IO.File o_file = new(s_testDirectory + "text.txt", false);
            Assert.That(o_file.FileContent, Is.EqualTo("Hello World!\r\nHello World!!\r\n"), "downloaded file content from '" + p_s_startingFolderRemote + "first_subfolder/test.txt' is not equal to uploaded bytes from before");

            p_o_sftpClient.Rename(p_s_startingFolderRemote + "first_subfolder/test.txt", p_s_startingFolderRemote + "renamed_test.txt").Wait();
            Assert.That(p_o_sftpClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt"), Is.True, "could not rename file '" + p_s_startingFolderRemote + "first_subfolder/test.txt' to '" + p_s_startingFolderRemote + "renamed_test.txt'");

            p_o_sftpClient.Rename(p_s_startingFolderRemote + "first_subfolder", p_s_startingFolderRemote + "delete_subfolder").Wait();
            Assert.That(p_o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder"), Is.True, "could not rename folder '" + p_s_startingFolderRemote + "first_subfolder' to '" + p_s_startingFolderRemote + "delete_subfolder'");

            p_o_sftpClient.Upload(s_resourcesDirectory + "1-MB-Test.xlsx", p_s_startingFolderRemote + "second_folder/singleFile.xlsx", false).Wait();
            Assert.That(p_o_sftpClient.FileExists(p_s_startingFolderRemote + "second_folder/singleFile.xlsx"), Is.True, "could not upload file '1-MB-Test.xlsx' to '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'");

            Assert.That(p_o_sftpClient.GetLength(p_s_startingFolderRemote + "second_folder/singleFile.xlsx"), Is.EqualTo(
                Convert.ToInt64(
                    (p_o_sftpClient.Download(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result ?? []).Length
                )
            ), "file length from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx' is not equal do downloaded bytes from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'");

            p_o_sftpClient.UploadFolder(s_resourcesDirectory, p_s_startingFolderRemote + "delete_subfolder/test", true).Wait();

            int i_countFiles = 0;
            int i_coundDirectories = 0;

            foreach (ForestNET.Lib.Net.FTP.Entry o_entry in p_o_sftpClient.Ls(p_s_startingFolderRemote + "delete_subfolder/test", false, false, false, true).Result)
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

            p_o_sftpClient.DownloadFolder(p_s_startingFolderRemote + "delete_subfolder", s_testDirectory + ForestNET.Lib.IO.File.DIR + "download_folder", true, true).Wait();

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
            long l_right = ForestNET.Lib.IO.File.FileLength(s_testDirectory + "download_folder" + ForestNET.Lib.IO.File.DIR + "test" + ForestNET.Lib.IO.File.DIR + "10-MB-Test.xlsx");

            Assert.That(l_left, Is.EqualTo(l_right), "file length of '10-MB-Test.xlsx' in upload sources and downloaded folder are not equal: '" + l_left + "' != '" + l_right + "'");

            p_o_sftpClient.Delete(p_s_startingFolderRemote + "renamed_test.txt").Wait();
            Assert.That(!p_o_sftpClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt"), Is.True, "could not delete file '" + p_s_startingFolderRemote + "renamed_test.txt'");

            p_o_sftpClient.RmDir(p_s_startingFolderRemote + "delete_subfolder").Wait();
            Assert.That(!p_o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder"), Is.True, "could not delete directory '" + p_s_startingFolderRemote + "delete_subfolder'");

            p_o_sftpClient.RmDir(p_s_startingFolderRemote + "second_folder").Wait();
            Assert.That(!p_o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "second_folder"), Is.True, "could not delete directory '" + p_s_startingFolderRemote + "second_folder'");

            Assert.That(p_o_sftpClient.Logout(), Is.True, "could not logout");

            ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                Is.False,
                "directory[" + s_testDirectory + "] does exist"
            );
        }
    }
}
