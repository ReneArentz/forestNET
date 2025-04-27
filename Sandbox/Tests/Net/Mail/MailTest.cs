namespace Sandbox.Tests.Net.Mail
{
    public class MailTest
    {
        private class MailLoginData
        {
            public string s_imapSmtpServer = "127.0.0.1";
            public int i_imapSmtpServerPort = 587;
            public string s_imapSmtpUser = "postmaster@mshome.net";
            public string s_imapSmtpPass = "postmaster";

            public string s_imapMailServer = "127.0.0.1";
            public int i_imapMailServerPort = 143;
            public string s_imapMailUserOne = "test1@mshome.net";
            public string s_imapMailPassOne = "Testtest1!";
            public string s_imapMailUserTwo = "test2@mshome.net";
            public string s_imapMailPassTwo = "Testtest1!";

            public ForestNET.Lib.Net.Mail.SecurityOptions e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            public ForestNET.Lib.Net.Mail.SecurityOptions e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.None;

            public string s_certificatePath = "";

            public MailLoginData(string s_resourcesDirectory, string p_s_imapServerIp)
            {
                s_imapSmtpServer = p_s_imapServerIp;
                s_imapMailServer = p_s_imapServerIp;
                s_certificatePath = s_resourcesDirectory + "certificate.crt";
            }
        }

        public static void TestMail(string p_s_mailServerIp)
        {
            try
            {
                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                //string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testMail" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "mail" + ForestNET.Lib.IO.File.DIR;

                //if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                //    ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                //ForestNET.Lib.IO.File.CreateDirectory(s_testDirectory);

                //if (!ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                //    throw new Exception("directory[" + s_testDirectory + "] does not exist");

                MailLoginData o_mailLoginData = new(s_resourcesDirectory, p_s_mailServerIp)
                {
                    e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls,
                    e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls,
                    i_imapSmtpServerPort = 587,
                    i_imapMailServerPort = 143
                };

                ForestNET.Lib.Net.Mail.Client<MailKit.Net.Imap.ImapClient> o_mailClient = new(null, o_mailLoginData.s_imapSmtpServer, 1, o_mailLoginData.i_imapSmtpServerPort, null, null, o_mailLoginData.s_imapSmtpUser, o_mailLoginData.s_imapSmtpPass, o_mailLoginData.e_securitySmtp);
                o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(o_mailLoginData.s_certificatePath));

                if (!o_mailClient.Login().Result)
                    throw new Exception("mail client could not login");

                ForestNET.Lib.Net.Mail.Message o_message = new("test1@mshome.net", "test2@mshome.net", "#1 To test1 and test2 plain text", "send from postmaster");
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Send message with plain text to 'test1@mshome.net' and 'test2@mshome.net'");

                o_message = new("test1@mshome.net", "#2 To test1 plain text", "send from postmaster");
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Send message with plain text to 'test1@mshome.net'");

                o_message = new("test1@mshome.net", "test2@mshome.net", "#3 To test1 and test2 plain text and html text", "*bold* /italic/" + ForestNET.Lib.IO.File.NEWLINE + "_underline_" + ForestNET.Lib.IO.File.NEWLINE + "send from postmaster")
                {
                    Html = "<html>\r\n"
                        + "  <head>\r\n"
                        + "    <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\">\r\n"
                        + "  </head>\r\n"
                        + "  <body>\r\n"
                        + "    <p><b>bold</b> <i>italic</i></p>\r\n"
                        + "    <p><u>underline</u><br>\r\n"
                        + "    </p>\r\n"
                        + "    <p>send from postmaster</p>\r\n"
                        + "  </body>\r\n"
                        + "</html>"
                };
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Send message with plain and html text to 'test1@mshome.net' and 'test2@mshome.net'");

                o_message = new("test1@mshome.net", "#4 To test1 plain text and html text", "*bold*" + ForestNET.Lib.IO.File.NEWLINE + "_underline_" + ForestNET.Lib.IO.File.NEWLINE + "send from postmaster")
                {
                    Html = "<html>\r\n"
                        + "  <head>\r\n"
                        + "    <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\">\r\n"
                        + "  </head>\r\n"
                        + "  <body>\r\n"
                        + "    <p><b>bold</b></p>\r\n"
                        + "    <p><u>underline</u><br>\r\n"
                        + "    </p>\r\n"
                        + "    <p>send from postmaster</p>\r\n"
                        + "  </body>\r\n"
                        + "</html>"
                };
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Send message with plain and html text to 'test1@mshome.net'");

                o_message = new ForestNET.Lib.Net.Mail.Message("test1@mshome.net;test2@mshome.net", "#5 To test1 and test2 plain text and html text and attachments", "*bold_attachments* /italic_attachments/" + ForestNET.Lib.IO.File.NEWLINE + "_underline_attachments_" + ForestNET.Lib.IO.File.NEWLINE + "send from postmaster")
                {
                    Html = "<html>\r\n"
                        + "  <head>\r\n"
                        + "    <meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\">\r\n"
                        + "  </head>\r\n"
                        + "  <body>\r\n"
                        + "    <p><b>bold_attachments</b> <i>italic_attachments</i></p>\r\n"
                        + "    <p><u>underline_attachments</u><br>\r\n"
                        + "    </p>\r\n"
                        + "    <p>send from postmaster</p>\r\n"
                        + "  </body>\r\n"
                        + "</html>"
                };
                o_message.AddAttachment(s_resourcesDirectory + "attachment1.txt");
                o_message.AddAttachment(s_resourcesDirectory + "attachment2.pdf");
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Send message with plain and html text and two attachments to 'test1@mshome.net' and 'test2@mshome.net'");

                if (!o_mailClient.Logout().Result)
                    throw new Exception("mail client could not logout");

                Thread.Sleep(1500);

                o_mailClient = new(o_mailLoginData.s_imapMailServer, o_mailLoginData.i_imapMailServerPort, o_mailLoginData.i_imapSmtpServerPort, o_mailLoginData.s_imapMailUserOne, o_mailLoginData.s_imapMailPassOne, o_mailLoginData.e_securityImap);
                o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(o_mailLoginData.s_certificatePath));

                if (!o_mailClient.Login().Result)
                    throw new Exception("mail client could not login");

                int i_messageAmountInbox = o_mailClient.GetMessagesAmount().Result;

                Console.WriteLine("Amount of total messages in imap(s) inbox: " + i_messageAmountInbox);

                if (i_messageAmountInbox != 5)
                    throw new Exception("amount of total messages in imap(s) inbox is not '5'");

                List<ForestNET.Lib.Net.Mail.Message> a_messages = o_mailClient.GetMessages(null, false, false, true).Result;

                string s_messageId_1 = "message id is null";
                string s_messageId_2 = "message id is null";
                string s_messageId_5 = "message id is null";

                foreach (ForestNET.Lib.Net.Mail.Message o_messageObj in a_messages)
                {
                    if (o_messageObj.Subject == null)
                    {
                        continue;
                    }

                    if (o_messageObj.Subject.Equals("#1 To test1 and test2 plain text"))
                    {
                        s_messageId_1 = o_messageObj.MessageId ?? "message id is null";
                    }
                    else if (o_messageObj.Subject.Equals("#2 To test1 plain text"))
                    {
                        s_messageId_2 = o_messageObj.MessageId ?? "message id is null";
                    }
                    else if (o_messageObj.Subject.Equals("#5 To test1 and test2 plain text and html text and attachments"))
                    {
                        s_messageId_5 = o_messageObj.MessageId ?? "message id is null";
                    }

                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine(o_messageObj.ToString(true));
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine("-------------------------------------------------------");
                    Console.WriteLine("-------------------------------------------------------");
                }

                o_message = o_mailClient.GetMessageById(s_messageId_1, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
                if (!(o_message.Subject ?? string.Empty).Equals("#1 To test1 and test2 plain text"))
                    throw new Exception("subject of message #1 has unexpected value");

                o_message = o_mailClient.GetMessageById(s_messageId_2, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
                if (!(o_message.Subject ?? string.Empty).Equals("#2 To test1 plain text"))
                    throw new Exception("subject of message #2 has unexpected value");

                o_message = o_mailClient.GetMessageById(s_messageId_5, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
                if (!(o_message.Subject ?? string.Empty).Equals("#5 To test1 and test2 plain text and html text and attachments"))
                    throw new Exception("subject of message #5 has unexpected value");

                if (o_message.HasAttachments)
                    throw new Exception("message #5 has attachments, although we ignore attachments with 'GetMessageById'");

                o_message = o_mailClient.GetMessageById(s_messageId_2, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
                o_message.From = null;
                o_message.To = "test2@mshome.net";
                o_message.Subject = "FW: " + o_message.Subject;
                o_message.Text = "A message from postmaster" + ForestNET.Lib.IO.File.NEWLINE + o_message.Text;
                o_mailClient.SendMessage(o_message).Wait();

                Console.WriteLine("Forwarded message with plain and html text to 'test2@mshome.net'");

                o_mailClient.ExpungeFolder().Wait();

                Console.WriteLine("Deleted all messages in 'INBOX' of 'test1@mshome.net'");

                o_mailClient.ExpungeFolder("Sent").Wait();

                Console.WriteLine("Deleted all messages in 'Sent' of 'test1@mshome.net'");

                if (!o_mailClient.Logout().Result)
                    throw new Exception("mail client could not logout");

                Thread.Sleep(2000);

                o_mailClient = new(o_mailLoginData.s_imapMailServer, o_mailLoginData.i_imapMailServerPort, o_mailLoginData.i_imapSmtpServerPort, o_mailLoginData.s_imapMailUserTwo, o_mailLoginData.s_imapMailPassTwo, o_mailLoginData.e_securityImap);
                o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(o_mailLoginData.s_certificatePath));

                if (!o_mailClient.Login().Result)
                    throw new Exception("mail client could not login");

                i_messageAmountInbox = o_mailClient.GetMessagesAmount().Result;

                Console.WriteLine("Amount of total messages in imap(s) inbox: " + i_messageAmountInbox);

                if (i_messageAmountInbox != 4)
                    throw new Exception("amount of total messages in imap(s) inbox is not '4'");

                o_mailClient.ExpungeFolder().Wait();

                Console.WriteLine("Deleted all messages in 'INBOX' of 'test2@mshome.net'");

                if (!o_mailClient.Logout().Result)
                    throw new Exception("mail client could not logout");

                //ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);

                //if (ForestNET.Lib.IO.File.FolderExists(s_testDirectory))
                //    throw new Exception("directory[" + s_testDirectory + "] does exist");
            }
            catch (Exception o_exc)
            {
                ForestNET.Lib.Global.LogException(o_exc);
            }
        }
    }
}
