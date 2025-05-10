namespace ForestNET.Tests.Net.Mail
{
    public class ClientUnitTest
    {
        [Test]
        public void TestClient()
        {
            try
            {
                /**
		         * !!!!!!!!!!!!!!!!!
		         * it is maybe necessary to create new certificate file, or rather update 'certificate.crt' in test resources
		         * !!!!!!!!!!!!!!!!!
		         */

                TestConfig.InitiateTestLogging();

                string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
                string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testMail" + ForestNET.Lib.IO.File.DIR;
                string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR + "mail" + ForestNET.Lib.IO.File.DIR;

                MailLoginData o_mailLoginData = new(s_resourcesDirectory, "172.24.87.179", "172.24.87.179");

                RunMail(s_testDirectory, s_resourcesDirectory, o_mailLoginData);
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private class MailLoginData
        {
            public string s_pop3SmtpServer = "127.0.0.1";
            public int i_pop3SmtpServerPort = 587;
            public string s_pop3SmtpUser = "postmaster@mshome.net";
            public string s_pop3SmtpPass = "postmaster";

            public string s_pop3MailServer = "127.0.0.1";
            public int i_pop3MailServerPort = 110;
            public string s_pop3MailUser = "test1@mshome.net";
            public string s_pop3MailPass = "Testtest1!";

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
            public ForestNET.Lib.Net.Mail.SecurityOptions e_securityPop3 = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            public ForestNET.Lib.Net.Mail.SecurityOptions e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.None;

            public string s_certificatePath = "";

            public MailLoginData(string p_s_resourcesDirectory, string p_s_pop3ServerIp, string p_s_imapServerIp)
            {
                s_pop3SmtpServer = p_s_pop3ServerIp;
                s_pop3MailServer = p_s_pop3ServerIp;
                s_imapSmtpServer = p_s_imapServerIp;
                s_imapMailServer = p_s_imapServerIp;
                s_certificatePath = p_s_resourcesDirectory + "certificate.crt";
            }
        }

        private static void RunMail(string p_s_testDirectory, string p_s_resourcesDirectory, MailLoginData p_o_mailLoginData)
        {
            if (ForestNET.Lib.IO.File.FolderExists(p_s_testDirectory))
            {
                ForestNET.Lib.IO.File.DeleteDirectory(p_s_testDirectory);
            }

            ForestNET.Lib.IO.File.CreateDirectory(p_s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(p_s_testDirectory),
                Is.True,
                "directory[" + p_s_testDirectory + "] does not exist"
            );

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            p_o_mailLoginData.e_securityPop3 = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            p_o_mailLoginData.i_pop3SmtpServerPort = 25;
            p_o_mailLoginData.i_pop3MailServerPort = 110;

            RunPop3(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment1.txt"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment1.txt");
            }

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment2.pdf"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment2.pdf");
            }

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls;
            p_o_mailLoginData.e_securityPop3 = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls;
            p_o_mailLoginData.i_pop3SmtpServerPort = 587;
            p_o_mailLoginData.i_pop3MailServerPort = 110;

            RunPop3(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment1.txt"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment1.txt");
            }

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment2.pdf"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment2.pdf");
            }

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.Tls;
            p_o_mailLoginData.e_securityPop3 = ForestNET.Lib.Net.Mail.SecurityOptions.Tls;
            p_o_mailLoginData.i_pop3SmtpServerPort = 465;
            p_o_mailLoginData.i_pop3MailServerPort = 995;

            RunPop3(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment1.txt"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment1.txt");
            }

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment2.pdf"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment2.pdf");
            }

            /* ###################################################################################### */

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            p_o_mailLoginData.e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.None;
            p_o_mailLoginData.i_imapSmtpServerPort = 25;
            p_o_mailLoginData.i_imapMailServerPort = 143;

            RunImap(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment1.txt"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment1.txt");
            }

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment2.pdf"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment2.pdf");
            }

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls;
            p_o_mailLoginData.e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.StartTls;
            p_o_mailLoginData.i_imapSmtpServerPort = 587;
            p_o_mailLoginData.i_imapMailServerPort = 143;

            RunImap(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment1.txt"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment1.txt");
            }

            if (ForestNET.Lib.IO.File.Exists(p_s_testDirectory + "attachment2.pdf"))
            {
                ForestNET.Lib.IO.File.DeleteFile(p_s_testDirectory + "attachment2.pdf");
            }

            p_o_mailLoginData.e_securitySmtp = ForestNET.Lib.Net.Mail.SecurityOptions.Tls;
            p_o_mailLoginData.e_securityImap = ForestNET.Lib.Net.Mail.SecurityOptions.Tls;
            p_o_mailLoginData.i_imapSmtpServerPort = 465;
            p_o_mailLoginData.i_imapMailServerPort = 993;

            RunImap(p_s_testDirectory, p_s_resourcesDirectory, p_o_mailLoginData);

            ForestNET.Lib.IO.File.DeleteDirectory(p_s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(p_s_testDirectory),
                Is.False,
                "directory[" + p_s_testDirectory + "] does exist"
            );
        }

        private static void RunPop3(string p_s_testDirectory, string p_s_resourcesDirectory, MailLoginData p_o_mailLoginData)
        {
            ForestNET.Lib.Net.Mail.Client<MailKit.Net.Pop3.Pop3Client> o_mailClient = new(p_o_mailLoginData.s_pop3SmtpServer, p_o_mailLoginData.i_pop3SmtpServerPort, p_o_mailLoginData.s_pop3SmtpUser, p_o_mailLoginData.s_pop3SmtpPass, p_o_mailLoginData.e_securitySmtp);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            ForestNET.Lib.Net.Mail.Message o_message = new("test1@mshome.net", "Test 1", "send from JUnit 5 Test");
            o_mailClient.SendMessage(o_message).Wait();

            o_message = new ForestNET.Lib.Net.Mail.Message("test1@mshome.net", "Test 2", "send from JUnit 5 Test, again");
            o_mailClient.SendMessage(o_message).Wait();

            o_message = new ForestNET.Lib.Net.Mail.Message("test1@mshome.net", "Test 3", "send from JUnit 5 Test, with attachments");
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment1.txt");
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment2.pdf");
            o_mailClient.SendMessage(o_message).Wait();

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Thread.Sleep(1000);

            o_mailClient = new(p_o_mailLoginData.s_pop3MailServer, null, p_o_mailLoginData.i_pop3MailServerPort, 1, p_o_mailLoginData.s_pop3MailUser, p_o_mailLoginData.s_pop3MailPass, null, null, p_o_mailLoginData.e_securityPop3);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            Assert.That(o_mailClient.GetMessagesAmount(true).Result, Is.EqualTo(3), "amount of unread messages in pop3(s) inbox is not '3'");
            Assert.That(o_mailClient.GetMessagesAmount().Result, Is.EqualTo(3), "amount of total messages in pop3(s) inbox is not '3'");

            List<ForestNET.Lib.Net.Mail.Message> a_messages = o_mailClient.GetMessages(null, true, false).Result;

            bool b_attachmentsFound = false;

            foreach (ForestNET.Lib.Net.Mail.Message o_messageObj in a_messages)
            {
                if (o_messageObj.Subject == null)
                {
                    Assert.Fail("message has no subject");
                    continue;
                }

                if (o_messageObj.Subject.Equals("Test 1"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test"), "mail text from message ist not 'send from JUnit 5 Test', but '" + o_messageObj.Text?.Trim() + "'");
                }
                else if (o_messageObj.Subject.Equals("Test 2"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test, again"), "mail text from message ist not 'send from JUnit 5 Test, again', but '" + o_messageObj.Text?.Trim() + "'");
                }
                else if (o_messageObj.Subject.Equals("Test 3"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test, with attachments"), "mail text from message ist not 'send from JUnit 5 Test, with attachments', but '" + o_messageObj.Text?.Trim() + "'");
                }

                if (o_messageObj.HasAttachments)
                {
                    for (int i = 0; i < o_messageObj.Attachments.Count; i++)
                    {
                        o_messageObj.SaveAllAttachments(p_s_testDirectory);
                        b_attachmentsFound = true;
                    }
                }
            }

            Assert.That(b_attachmentsFound, Is.True, "no attachments found after reading all messages from pop3(s) inbox");

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Assert.That(
                ForestNET.Lib.IO.File.HashFile(p_s_resourcesDirectory + "attachment1.txt", "SHA-256"),
                Is.EqualTo(ForestNET.Lib.IO.File.HashFile(p_s_testDirectory + "attachment1.txt", "SHA-256")),
                "hash values of resource and test attachment 'attachment1.txt' do not match to each other"
            );

            Assert.That(
                ForestNET.Lib.IO.File.HashFile(p_s_resourcesDirectory + "attachment2.pdf", "SHA-256"),
                Is.EqualTo(ForestNET.Lib.IO.File.HashFile(p_s_testDirectory + "attachment2.pdf", "SHA-256")),
                "hash values of resource and test attachment 'attachment2.pdf' do not match to each other"
            );
        }

        private static void RunImap(string p_s_testDirectory, string p_s_resourcesDirectory, MailLoginData p_o_mailLoginData)
        {
            ForestNET.Lib.Net.Mail.Client<MailKit.Net.Imap.ImapClient> o_mailClient = new(p_o_mailLoginData.s_imapSmtpServer, p_o_mailLoginData.i_imapSmtpServerPort, p_o_mailLoginData.s_imapSmtpUser, p_o_mailLoginData.s_imapSmtpPass, p_o_mailLoginData.e_securitySmtp);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            ForestNET.Lib.Net.Mail.Message o_message = new("test1@mshome.net", "Test 1", "send from JUnit 5 Test");
            o_mailClient.SendMessage(o_message).Wait();

            o_message = new ForestNET.Lib.Net.Mail.Message("test1@mshome.net", "Test 2", "send from JUnit 5 Test, again");
            o_mailClient.SendMessage(o_message).Wait();

            o_message = new ForestNET.Lib.Net.Mail.Message("test1@mshome.net", "Test 3", "send from JUnit 5 Test, with attachments");
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment1.txt");
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment2.pdf");
            o_mailClient.SendMessage(o_message).Wait();

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Thread.Sleep(1500);

            o_mailClient = new(p_o_mailLoginData.s_imapMailServer, null, p_o_mailLoginData.i_imapMailServerPort, 1, p_o_mailLoginData.s_imapMailUserOne, p_o_mailLoginData.s_imapMailPassOne, null, null, p_o_mailLoginData.e_securityImap);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            List<ForestNET.Lib.Net.Mail.Message> a_messages = o_mailClient.GetMessages(null, false, false, true).Result;

            Assert.That(o_mailClient.GetMessagesAmount(true).Result, Is.EqualTo(0), "amount of unread messages in imap(s) inbox is not '0'");
            Assert.That(o_mailClient.GetMessagesAmount().Result, Is.EqualTo(3), "amount of total messages in imap(s) inbox is not '3'");

            bool b_attachmentsFound = false;

            foreach (ForestNET.Lib.Net.Mail.Message o_messageObj in a_messages)
            {
                if (o_messageObj.Subject == null)
                {
                    Assert.Fail("message has no subject");
                    continue;
                }

                if (o_messageObj.Subject.Equals("Test 1"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test"), "mail text from message ist not 'send from JUnit 5 Test', but '" + o_messageObj.Text?.Trim() + "'");
                }
                else if (o_messageObj.Subject.Equals("Test 2"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test, again"), "mail text from message ist not 'send from JUnit 5 Test, again', but '" + o_messageObj.Text?.Trim() + "'");
                }
                else if (o_messageObj.Subject.Equals("Test 3"))
                {
                    Assert.That(o_messageObj.Text?.Trim(), Is.EqualTo("send from JUnit 5 Test, with attachments"), "mail text from message ist not 'send from JUnit 5 Test, with attachments', but '" + o_messageObj.Text?.Trim() + "'");
                }

                if (o_messageObj.HasAttachments)
                {
                    for (int i = 0; i < o_messageObj.Attachments.Count; i++)
                    {
                        o_messageObj.SaveAllAttachments(p_s_testDirectory);
                        b_attachmentsFound = true;
                    }
                }
            }

            Assert.That(b_attachmentsFound, Is.True, "no attachments found after reading all messages from imap(s) inbox");

            Assert.That(
                ForestNET.Lib.IO.File.HashFile(p_s_resourcesDirectory + "attachment1.txt", "SHA-256"),
                Is.EqualTo(ForestNET.Lib.IO.File.HashFile(p_s_testDirectory + "attachment1.txt", "SHA-256")),
                "hash values of resource and test attachment 'attachment1.txt' do not match to each other"
            );

            Assert.That(
                ForestNET.Lib.IO.File.HashFile(p_s_resourcesDirectory + "attachment2.pdf", "SHA-256"),
                Is.EqualTo(ForestNET.Lib.IO.File.HashFile(p_s_testDirectory + "attachment2.pdf", "SHA-256")),
                "hash values of resource and test attachment 'attachment2.pdf' do not match to each other"
            );

            _ = o_mailClient.GetMessages(null, true, true, false).Result;

            Assert.That(o_mailClient.GetMessagesAmount(true).Result, Is.EqualTo(0), "amount of unread messages in imap(s) inbox is not '0'");
            Assert.That(o_mailClient.GetMessagesAmount().Result, Is.EqualTo(0), "amount of total messages in imap(s) inbox is not '0'");

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Thread.Sleep(1500);

            o_mailClient = new(null, p_o_mailLoginData.s_imapSmtpServer, 1, p_o_mailLoginData.i_imapSmtpServerPort, null, null, p_o_mailLoginData.s_imapSmtpUser, p_o_mailLoginData.s_imapSmtpPass, p_o_mailLoginData.e_securitySmtp);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            o_message = new("test1@mshome.net", "test2@mshome.net", "#1 To test1 and test2 plain text", "send from postmaster");
            o_mailClient.SendMessage(o_message).Wait();

            o_message = new("test1@mshome.net", "#2 To test1 plain text", "send from postmaster");
            o_mailClient.SendMessage(o_message).Wait();

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
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment1.txt");
            o_message.AddAttachment(p_s_resourcesDirectory + "attachment2.pdf");
            o_mailClient.SendMessage(o_message).Wait();

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Thread.Sleep(1500);

            o_mailClient = new(p_o_mailLoginData.s_imapMailServer, p_o_mailLoginData.i_imapMailServerPort, p_o_mailLoginData.i_imapSmtpServerPort, p_o_mailLoginData.s_imapMailUserOne, p_o_mailLoginData.s_imapMailPassOne, p_o_mailLoginData.e_securityImap);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            Assert.That(o_mailClient.GetMessagesAmount().Result, Is.EqualTo(5), "amount of total messages in imap(s) inbox is not '5'");

            a_messages = o_mailClient.GetMessages(null, false, false, true).Result;

            string s_messageId_1 = "message id is null";
            string s_messageId_2 = "message id is null";
            string s_messageId_5 = "message id is null";

            foreach (ForestNET.Lib.Net.Mail.Message o_messageObj in a_messages)
            {
                if (o_messageObj.Subject == null)
                {
                    Assert.Fail("message has no subject");
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
            }

            o_message = o_mailClient.GetMessageById(s_messageId_1, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
            Assert.That(o_message.Subject, Is.EqualTo("#1 To test1 and test2 plain text"), "subject of message #1 has unexpected value");

            o_message = o_mailClient.GetMessageById(s_messageId_2, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
            Assert.That(o_message.Subject, Is.EqualTo("#2 To test1 plain text"), "subject of message #2 has unexpected value");

            o_message = o_mailClient.GetMessageById(s_messageId_5, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
            Assert.That(o_message.Subject, Is.EqualTo("#5 To test1 and test2 plain text and html text and attachments"), "subject of message #5 has unexpected value");
            Assert.That(o_message.HasAttachments, Is.False, "message #5 has attachments, although we ignore attachments with 'GetMessageById'");

            o_message = o_mailClient.GetMessageById(s_messageId_2, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
            o_message.From = null;
            o_message.To = "test2@mshome.net";
            o_message.Subject = "FW: " + o_message.Subject;
            o_message.Text = "A message from postmaster" + ForestNET.Lib.IO.File.NEWLINE + o_message.Text;
            o_mailClient.SendMessage(o_message).Wait();

            o_mailClient.CreateSubFolder("subfolder").Wait();

            o_mailClient.ChangeToFolder("subfolder").Wait();

            o_mailClient.CreateSubFolder("another_subfolder").Wait();

            o_mailClient.ChangeToFolder("..").Wait();

            o_mailClient.MoveMessages([s_messageId_1, s_messageId_5], "INBOX/subfolder/another_subfolder").Wait();

            o_mailClient.ChangeToFolder("subfolder").Wait();

            o_mailClient.RenameSubFolder("another_subfolder", "sub").Wait();

            o_mailClient.ChangeToFolder("sub").Wait();

            o_mailClient.MoveAllMessages("INBOX/subfolder", "FLAGGED", true).Wait();

            o_mailClient.DeleteFolder().Wait();

            o_mailClient.SetSeen(s_messageId_1).Wait();

            o_message = o_mailClient.GetMessageById(s_messageId_1, true).Result ?? throw new NullReferenceException("message result of 'GetMessageById' is null");
            Assert.That(o_message.HasFlag("FLAGGED"), Is.True, "message #1 has not the FLAGGED flag");
            Assert.That(o_message.HasFlag("SEEN"), Is.True, "message #1 has not the SEEN flag");

            o_mailClient.ExpungeFolder().Wait();

            o_mailClient.ExpungeFolder("INBOX").Wait();

            o_mailClient.ExpungeFolder("Sent").Wait();

            o_mailClient.DeleteFolder().Wait();

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");

            Thread.Sleep(1000);

            o_mailClient = new(p_o_mailLoginData.s_imapMailServer, p_o_mailLoginData.i_imapMailServerPort, p_o_mailLoginData.i_imapSmtpServerPort, p_o_mailLoginData.s_imapMailUserTwo, p_o_mailLoginData.s_imapMailPassTwo, p_o_mailLoginData.e_securityImap);
            o_mailClient.AddCertificateToAllowList(new System.Security.Cryptography.X509Certificates.X509Certificate2(p_o_mailLoginData.s_certificatePath));

            Assert.That(o_mailClient.Login().Result, Is.True, "mail client could not login");

            Assert.That(o_mailClient.GetMessagesAmount().Result, Is.EqualTo(4), "amount of total messages in imap(s) inbox is not '4'");

            o_mailClient.ExpungeFolder().Wait();

            Assert.That(o_mailClient.Logout().Result, Is.True, "mail client could not logout");
        }
    }
}
