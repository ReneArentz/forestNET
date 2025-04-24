namespace Sandbox.Tests.Net.SFTP
{
    public class SFTPTest
    {
        /**
		 * !!!!!!!!!!!!!!!!!
		 * it is maybe necessary to create new authentication and known_hosts files
		 * known_hosts entry must be base64 string of SHA-256 fingerprint
		 * !!!!!!!!!!!!!!!!!
		 */

        public static void TestSFTP(string p_s_sftpHostIp, int p_i_sftpPort, string p_s_sftpUser, string p_s_startingFolderRemote)
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testSftp" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesSFTPDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "sftp" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ftp" + ForestNET.Lib.IO.File.DIR;

                ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

                string s_ftpUrlPrefix = "sftp://";
                string s_ftpUrl = s_ftpUrlPrefix + p_s_sftpHostIp;
                ForestNET.Lib.Net.SFTP.Client o_sftpClient = new(s_ftpUrl, p_i_sftpPort, p_s_sftpUser, s_resourcesSFTPDirectory + "sftp_id_rsa", null, s_resourcesSFTPDirectory + "known_hosts")
                {
                    Encoding = System.Text.Encoding.UTF8,
                };

                ForestNET.Lib.Net.SFTP.Client.PostProgress del_postProgress = (double p_d_progress) =>
                {
                    o_consoleProgressBar.Report = p_d_progress;
                };

                o_sftpClient.DelegatePostProgress = del_postProgress;

                ForestNET.Lib.Net.SFTP.Client.PostProgressFolder del_postProgressFolder = (int p_i_filesProcessed, int p_i_files) =>
                {
                    Console.WriteLine(p_i_filesProcessed + "/" + p_i_files);
                };

                o_sftpClient.DelegatePostProgressFolder = del_postProgressFolder;

                o_consoleProgressBar.Init();
                o_consoleProgressBar.Close();

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);

                if (!ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    throw new Exception("directory[" + s_testDirectory + "] does not exist");

                if (o_sftpClient == null)
                    throw new Exception("sftp client object is null");

                if (!o_sftpClient.Login().Result)
                    throw new Exception("could not login");

                o_sftpClient.MkDir(p_s_startingFolderRemote + "second_folder");
                if (!o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "second_folder"))
                    throw new Exception("folder '" + p_s_startingFolderRemote + "second_folder' could not be created");

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                o_sftpClient.Upload(o_sftpClient.Encoding.GetBytes("Hello World!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", false).Wait();
                if (!o_sftpClient.FileExists(p_s_startingFolderRemote + "first_subfolder/test.txt"))
                    throw new Exception("could not upload bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                o_sftpClient.Upload(o_sftpClient.Encoding.GetBytes("Hello World!!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", true).Wait();
                if (!o_sftpClient.FileExists(p_s_startingFolderRemote + "first_subfolder/test.txt"))
                    throw new Exception("could not append bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Download bytes . . .", "Done.");
                o_sftpClient.Download(p_s_startingFolderRemote + "first_subfolder/test.txt", s_testDirectory + "text.txt").Wait();
                if (!ForestNET.Lib.IO.File.Exists(s_testDirectory + "text.txt"))
                    throw new Exception("could not download file from '" + p_s_startingFolderRemote + "first_subfolder/test.txt'");
                o_consoleProgressBar.Close();

                ForestNET.Lib.IO.File o_file = new(s_testDirectory + "text.txt", false);
                if (!o_file.FileContent.Equals("Hello World!\r\nHello World!!\r\n"))
                    throw new Exception("downloaded file content from '" + p_s_startingFolderRemote + "first_subfolder/test.txt' is not equal to uploaded bytes from before");

                o_sftpClient.Rename(p_s_startingFolderRemote + "first_subfolder/test.txt", p_s_startingFolderRemote + "renamed_test.txt").Wait();
                if (!o_sftpClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt"))
                    throw new Exception("could not rename file '" + p_s_startingFolderRemote + "first_subfolder/test.txt' to '" + p_s_startingFolderRemote + "renamed_test.txt'");

                o_sftpClient.Rename(p_s_startingFolderRemote + "first_subfolder", p_s_startingFolderRemote + "delete_subfolder").Wait();
                if (!o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder"))
                    throw new Exception("could not rename folder '" + p_s_startingFolderRemote + "first_subfolder' to '" + p_s_startingFolderRemote + "delete_subfolder'");

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                o_sftpClient.Upload(s_resourcesDirectory + "1-MB-Test.xlsx", p_s_startingFolderRemote + "second_folder/singleFile.xlsx", false).Wait();
                if (!o_sftpClient.FileExists(p_s_startingFolderRemote + "second_folder/singleFile.xlsx"))
                    throw new Exception("could not upload file '1-MB-Test.xlsx' to '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Download bytes . . .", "Done.");
                if (o_sftpClient.GetLength(p_s_startingFolderRemote + "second_folder/singleFile.xlsx") !=
                    Convert.ToInt64(
                        (o_sftpClient.Download(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result ?? []).Length
                    )
                )
                    throw new Exception("file length from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx' is not equal do downloaded bytes from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'");
                o_consoleProgressBar.Close();

                o_sftpClient.UploadFolder(s_resourcesDirectory, p_s_startingFolderRemote + "delete_subfolder/test", true).Wait();

                int i_countFiles = 0;
                int i_coundDirectories = 0;

                foreach (ForestNET.Lib.Net.FTP.Entry o_entry in o_sftpClient.Ls(p_s_startingFolderRemote + "delete_subfolder/test", false, false, false, true).Result)
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

                if (i_coundDirectories != 1)
                    throw new Exception("found remote directories under '" + p_s_startingFolderRemote + "delete_subfolder/test' are not '1', but '" + i_coundDirectories + "'");

                if (i_countFiles != 6)
                    throw new Exception("found remote files under '" + p_s_startingFolderRemote + "delete_subfolder/test' are not '6', but '" + i_countFiles + "'");

                o_sftpClient.DownloadFolder(p_s_startingFolderRemote + "delete_subfolder", s_testDirectory + ForestNET.Lib.IO.File.DIR + "download_folder", true, true).Wait();

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

                if (i_coundDirectories != 1)
                    throw new Exception("found local directories under './download_folder' are not '1', but '" + i_coundDirectories + "'");

                if (i_countFiles != 6)
                    throw new Exception("found local files under './download_folder' are not '6', but '" + i_countFiles + "'");

                long l_left = ForestNET.Lib.IO.File.FileLength(s_resourcesDirectory + "10-MB-Test.xlsx");
                long l_right = ForestNET.Lib.IO.File.FileLength(s_testDirectory + "download_folder" + ForestNET.Lib.IO.File.DIR + "test" + ForestNET.Lib.IO.File.DIR + "10-MB-Test.xlsx");

                if (l_left != l_right)
                    throw new Exception("file length of '10-MB-Test.xlsx' in upload sources and downloaded folder are not equal: '" + l_left + "' != '" + l_right + "'");

                o_sftpClient.Delete(p_s_startingFolderRemote + "renamed_test.txt").Wait();
                if (o_sftpClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt"))
                    throw new Exception("could not delete file '" + p_s_startingFolderRemote + "renamed_test.txt'");

                o_sftpClient.RmDir(p_s_startingFolderRemote + "delete_subfolder").Wait();
                if (o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder"))
                    throw new Exception("could not delete directory '" + p_s_startingFolderRemote + "delete_subfolder'");

                o_sftpClient.RmDir(p_s_startingFolderRemote + "second_folder").Wait();
                if (o_sftpClient.DirectoryExists(p_s_startingFolderRemote + "second_folder"))
                    throw new Exception("could not delete directory '" + p_s_startingFolderRemote + "second_folder'");

                if (!o_sftpClient.Logout())
                    throw new Exception("could not logout");

                ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    throw new Exception("directory[" + s_testDirectory + "] does exist");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }
        }
    }
}
