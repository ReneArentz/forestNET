namespace Sandbox.Tests.Net.FTPS
{
    public class FTPSTest
    {
        /**
		 * !!!!!!!!!!!!!!!!!
		 * it is maybe necessary to create new certificate file, or rather update 'proftpd.crt' in test resources
		 * !!!!!!!!!!!!!!!!!
		 */
        private static readonly string PathToFTPSCertificate = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? ">> cannot find current directory <<")
            + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ftps" + ForestNET.Lib.IO.File.DIR
            + "proftpd.crt";

        public static void TestFTPS(string p_s_ftpsHostIp, int p_i_ftpsPort, string p_s_ftpsUser, string p_s_ftpsPassword, string p_s_startingFolderRemote)
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testFtps" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "ftp" + ForestNET.Lib.IO.File.DIR;

                string s_ftpsUrlPrefix = "ftps://";
                string s_ftpsUrl;

                ForestNET.Lib.ConsoleProgressBar o_consoleProgressBar = new();

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

                ForestNET.Lib.Net.FTP.Client.PostProgress del_postProgress = (double p_d_progress) =>
                {
                    o_consoleProgressBar.Report = p_d_progress;
                };

                o_ftpsClient.DelegatePostProgress = del_postProgress;

                ForestNET.Lib.Net.FTP.Client.PostProgressFolder del_postProgressFolder = (int p_i_filesProcessed, int p_i_files) =>
                {
                    Console.WriteLine(p_i_filesProcessed + "/" + p_i_files);
                };

                o_ftpsClient.DelegatePostProgressFolder = del_postProgressFolder;

                o_consoleProgressBar.Init();
                o_consoleProgressBar.Close();

                if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);

                if (!ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                    throw new Exception("directory[" + s_testDirectory + "] does not exist");

                if (o_ftpsClient == null)
                    throw new Exception("ftp(s) client object is null");

                if (!o_ftpsClient.Login().Result)
                    throw new Exception("could not login; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("257"))
                    throw new Exception("ftp reply code is not '257', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.MkDir(p_s_startingFolderRemote + "second_folder").Result)
                    throw new Exception("folder '" + p_s_startingFolderRemote + "second_folder' could not be created; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("200"))
                    throw new Exception("ftp reply code is not '200', but '" + o_ftpsClient.FtpReplyCode + "'");

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                if (!o_ftpsClient.Upload(o_ftpsClient.Encoding.GetBytes("Hello World!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", false).Result)
                    throw new Exception("could not upload bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("150"))
                    throw new Exception("ftp reply code is not '150', but '" + o_ftpsClient.FtpReplyCode + "'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                if (!o_ftpsClient.Upload(o_ftpsClient.Encoding.GetBytes("Hello World!!\r\n"), p_s_startingFolderRemote + "first_subfolder/test.txt", true).Result)
                    throw new Exception("could not append bytes to '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("150"))
                    throw new Exception("ftp reply code is not '150', but '" + o_ftpsClient.FtpReplyCode + "'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Download bytes . . .", "Done.");
                if (!o_ftpsClient.Download(p_s_startingFolderRemote + "first_subfolder/test.txt", s_testDirectory + "text.txt").Result)
                    throw new Exception("could not download file from '" + p_s_startingFolderRemote + "first_subfolder/test.txt'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("150"))
                    throw new Exception("ftp reply code is not '150', but '" + o_ftpsClient.FtpReplyCode + "'");
                o_consoleProgressBar.Close();

                ForestNET.Lib.IO.File o_file = new(s_testDirectory + "text.txt", false);
                if (!o_file.FileContent.Equals("Hello World!\r\nHello World!!\r\n"))
                    throw new Exception("downloaded file content from '" + p_s_startingFolderRemote + "first_subfolder/test.txt' is not equal to uploaded bytes from before");

                if (!o_ftpsClient.Rename(p_s_startingFolderRemote + "first_subfolder/test.txt", p_s_startingFolderRemote + "renamed_test.txt").Result)
                    throw new Exception("could not rename file '" + p_s_startingFolderRemote + "first_subfolder/test.txt' to '" + p_s_startingFolderRemote + "renamed_test.txt'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("250"))
                    throw new Exception("ftp reply code is not '250', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.Rename(p_s_startingFolderRemote + "first_subfolder", p_s_startingFolderRemote + "delete_subfolder").Result)
                    throw new Exception("could not rename folder '" + p_s_startingFolderRemote + "first_subfolder' to '" + p_s_startingFolderRemote + "delete_subfolder'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("250"))
                    throw new Exception("ftp reply code is not '250', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.DirectoryExists(p_s_startingFolderRemote + "delete_subfolder").Result)
                    throw new Exception("directory '" + p_s_startingFolderRemote + "delete_subfolder' does not exist; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("226"))
                    throw new Exception("ftp reply code is not '226', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.FileExists(p_s_startingFolderRemote + "renamed_test.txt").Result)
                    throw new Exception("file '" + p_s_startingFolderRemote + "renamed_test.txt' does not exist; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("226"))
                    throw new Exception("ftp reply code is not '226', but '" + o_ftpsClient.FtpReplyCode + "'");

                o_consoleProgressBar.Init("Upload bytes . . .", "Done.");
                if (!o_ftpsClient.Upload(s_resourcesDirectory + "1-MB-Test.xlsx", p_s_startingFolderRemote + "second_folder/singleFile.xlsx", false).Result)
                    throw new Exception("could not upload file '1-MB-Test.xlsx' to '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("150"))
                    throw new Exception("ftp reply code is not '150', but '" + o_ftpsClient.FtpReplyCode + "'");
                o_consoleProgressBar.Close();

                o_consoleProgressBar.Init("Download bytes . . .", "Done.");
                if (o_ftpsClient.GetLength(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result !=
                    Convert.ToInt64(
                        (o_ftpsClient.Download(p_s_startingFolderRemote + "second_folder/singleFile.xlsx").Result ?? []).Length
                    )
                )
                    throw new Exception("file length from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx' is not equal do downloaded bytes from '" + p_s_startingFolderRemote + "second_folder/singleFile.xlsx'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("150"))
                    throw new Exception("ftp reply code is not '150', but '" + o_ftpsClient.FtpReplyCode + "'");
                o_consoleProgressBar.Close();

                o_ftpsClient.UploadFolder(s_resourcesDirectory, p_s_startingFolderRemote + "delete_subfolder/test", true).Wait();

                int i_countFiles = 0;
                int i_coundDirectories = 0;

                foreach (ForestNET.Lib.Net.FTP.Entry o_entry in o_ftpsClient.Ls(p_s_startingFolderRemote + "delete_subfolder/test", false, false, o_ftpsClient.PreferMLSD, true).Result)
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

                o_ftpsClient.DownloadFolder(p_s_startingFolderRemote + "delete_subfolder", s_testDirectory + ForestNET.Lib.IO.File.DIR + "download_folder", true, true).Wait();

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

                /* expect other file length if we not using binary mode */
                if (!o_ftpsClient.UseBinary)
                {
                    l_left = 9565856;
                }

                long l_right = ForestNET.Lib.IO.File.FileLength(s_testDirectory + "download_folder" + ForestNET.Lib.IO.File.DIR + "test" + ForestNET.Lib.IO.File.DIR + "10-MB-Test.xlsx");

                if (l_left != l_right)
                    throw new Exception("file length of '10-MB-Test.xlsx' in upload sources and downloaded folder are not equal: '" + l_left + "' != '" + l_right + "'");

                if (!o_ftpsClient.Delete(p_s_startingFolderRemote + "renamed_test.txt").Result)
                    throw new Exception("could not delete file '" + p_s_startingFolderRemote + "renamed_test.txt'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("250"))
                    throw new Exception("ftp reply code is not '250', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.RmDir(p_s_startingFolderRemote + "delete_subfolder").Result)
                    throw new Exception("could not delete directory '" + p_s_startingFolderRemote + "delete_subfolder'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("200"))
                    throw new Exception("ftp reply code is not '200', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.RmDir(p_s_startingFolderRemote + "second_folder").Result)
                    throw new Exception("could not delete directory '" + p_s_startingFolderRemote + "second_folder'; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("200"))
                    throw new Exception("ftp reply code is not '200', but '" + o_ftpsClient.FtpReplyCode + "'");

                if (!o_ftpsClient.Logout().Result)
                    throw new Exception("could not logout; " + o_ftpsClient.FtpReply);

                if (!(o_ftpsClient.FtpReplyCode ?? "0").Equals("221"))
                    throw new Exception("ftp reply code is not '221', but '" + o_ftpsClient.FtpReplyCode + "'");

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
