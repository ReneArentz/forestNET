namespace ForestNET.Tests.Net.Request
{
    public class ClientUnitTest
    {
        /**
	     * if this unit test fails completely, there is an issue with https://httpbin.org/ or no outgoing internet connection
	     * maybe httpbin must be run locally to run the unit test
	     * maybe the used proxy does not exist anymore
	     * 
	     * there is an explicit switch, if logging should be used or not. Request.setUseLog(boolean):
	     * somehow I encounter a state where active logging leads to ssl handshake exception
	     */
        [Test]
        public void TestClient()
        {
            try
            {
                TestConfig.InitiateTestLogging();

                string s_httpBinUrl = "https://httpbin.org/";
                string? s_proxyAddress = null;
                int i_proxyPort = 80;

                RunHttpRequests(s_httpBinUrl, s_proxyAddress, i_proxyPort, true, false); /* using no proxy */

                /*s_proxyAddress = "35.185.196.38";
                i_proxyPort = 3128;

                RunHttpRequests(s_httpBinUrl, s_proxyAddress, i_proxyPort, false, false); /* using proxy */
            }
            catch (Exception o_exc)
            {
                Assert.Fail(o_exc.ToString() + Environment.NewLine + System.Environment.StackTrace);
            }
        }

        private static void RunHttpRequests(string s_httpBinUrl, string? s_proxyAddress, int i_proxyPort, bool p_b_useDefaultCredentials, bool p_b_useLog)
        {
            string s_currentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'");
            string s_testDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "testRequest" + ForestNET.Lib.IO.File.DIR;
            string s_resourcesDirectory = s_currentDirectory + ForestNET.Lib.IO.File.DIR + "Resources" + ForestNET.Lib.IO.File.DIR;
            string s_attachmentFile = s_resourcesDirectory + "net" + ForestNET.Lib.IO.File.DIR + "request" + ForestNET.Lib.IO.File.DIR + "products.json";

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

            /* ******** */
            /* 1st test */
            /* ******** */

            ForestNET.Lib.Net.Request.Client o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "get")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            string s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            Assert.That(s_response, Does.Contain("\"args\": {}"), "Response message does not contain '\"args\": {}'");

            /* ******** */
            /* 2nd test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "get")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            o_requestClient.AddRequestParameter("param_1", "Hello World!");
            o_requestClient.AddRequestParameter("param_2", 1234.56d.ToString(System.Globalization.CultureInfo.InvariantCulture));

            s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            Assert.That(s_response, Does.Contain("\"param_1\": \"Hello World!\""), "Response message does not contain '\"param_1\": \"Hello World!\"'");
            Assert.That(s_response, Does.Contain("\"param_2\": \"1234.56\""), "Response message does not contain '\"param_2\": \"1234.56\"'");

            /* ******** */
            /* 3rd test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "get")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            o_requestClient.AddRequestParameter("param_1", "Hello World!");
            o_requestClient.AddRequestParameter("param_2", 1234.56d.ToString(System.Globalization.CultureInfo.InvariantCulture));
            o_requestClient.AddRequestParameter("param_3", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.");

            s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            Assert.That(s_response, Does.Contain("\"param_1\": \"Hello World!\""), "Response message does not contain '\"param_1\": \"Hello World!\"'");
            Assert.That(s_response, Does.Contain("\"param_2\": \"1234.56\""), "Response message does not contain '\"param_2\": \"1234.56\"'");
            Assert.That(s_response, Does.Contain("\"param_3\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""), "Response message does not contain '\"param_3\": \"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\"'");

            /* ******** */
            /* 4th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.POST, s_httpBinUrl + "post", ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS)
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            o_requestClient.AddAttachement("file1", s_attachmentFile);

            s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            string s_foo = "\"file1\": \"[\\r\\n  {\\r\\n    \\\"ProductID\\\": 1,\\r\\n    \\\"ProductName\\\": \\\"Chais\\\",\\r\\n    \\\"SupplierID\\\": 1,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"10 boxes x 20 bags\\\",\\r\\n    \\\"Price\\\": 18\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 2,\\r\\n    \\\"ProductName\\\": \\\"Chang\\\",\\r\\n    \\\"SupplierID\\\": 1,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 12 oz bottles\\\",\\r\\n    \\\"Price\\\": 19\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 3,\\r\\n    \\\"ProductName\\\": \\\"Aniseed Syrup\\\",\\r\\n    \\\"SupplierID\\\": 1,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"12 - 550 ml bottles\\\",\\r\\n    \\\"Price\\\": 10\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 4,\\r\\n    \\\"ProductName\\\": \\\"Chef Anton's Cajun Seasoning\\\",\\r\\n    \\\"SupplierID\\\": 2,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"48 - 6 oz jars\\\",\\r\\n    \\\"Price\\\": 22\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 5,\\r\\n    \\\"ProductName\\\": \\\"Chef Anton's Gumbo Mix\\\",\\r\\n    \\\"SupplierID\\\": 2,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"36 boxes\\\",\\r\\n    \\\"Price\\\": 21.35\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 6,\\r\\n    \\\"ProductName\\\": \\\"Grandma's Boysenberry Spread\\\",\\r\\n    \\\"SupplierID\\\": 3,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"12 - 8 oz jars\\\",\\r\\n    \\\"Price\\\": 25\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 7,\\r\\n    \\\"ProductName\\\": \\\"Uncle Bob's Organic Dried Pears\\\",\\r\\n    \\\"SupplierID\\\": 3,\\r\\n    \\\"CategoryID\\\": 7,\\r\\n    \\\"Unit\\\": \\\"12 - 1 lb pkgs.\\\",\\r\\n    \\\"Price\\\": 30\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 8,\\r\\n    \\\"ProductName\\\": \\\"Northwoods Cranberry Sauce\\\",\\r\\n    \\\"SupplierID\\\": 3,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"12 - 12 oz jars\\\",\\r\\n    \\\"Price\\\": 40\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 9,\\r\\n    \\\"ProductName\\\": \\\"Mishi Kobe Niku\\\",\\r\\n    \\\"SupplierID\\\": 4,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"18 - 500 g pkgs.\\\",\\r\\n    \\\"Price\\\": 97\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 10,\\r\\n    \\\"ProductName\\\": \\\"Ikura\\\",\\r\\n    \\\"SupplierID\\\": 4,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"12 - 200 ml jars\\\",\\r\\n    \\\"Price\\\": 31\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 11,\\r\\n    \\\"ProductName\\\": \\\"Queso Cabrales\\\",\\r\\n    \\\"SupplierID\\\": 5,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"1 kg pkg.\\\",\\r\\n    \\\"Price\\\": 21\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 12,\\r\\n    \\\"ProductName\\\": \\\"Queso Manchego La Pastora\\\",\\r\\n    \\\"SupplierID\\\": 5,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"10 - 500 g pkgs.\\\",\\r\\n    \\\"Price\\\": 38\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 13,\\r\\n    \\\"ProductName\\\": \\\"Konbu\\\",\\r\\n    \\\"SupplierID\\\": 6,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"2 kg box\\\",\\r\\n    \\\"Price\\\": 6\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 14,\\r\\n    \\\"ProductName\\\": \\\"Tofu\\\",\\r\\n    \\\"SupplierID\\\": 6,\\r\\n    \\\"CategoryID\\\": 7,\\r\\n    \\\"Unit\\\": \\\"40 - 100 g pkgs.\\\",\\r\\n    \\\"Price\\\": 23.25\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 15,\\r\\n    \\\"ProductName\\\": \\\"Genen Shouyu\\\",\\r\\n    \\\"SupplierID\\\": 6,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"24 - 250 ml bottles\\\",\\r\\n    \\\"Price\\\": 15.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 16,\\r\\n    \\\"ProductName\\\": \\\"Pavlova\\\",\\r\\n    \\\"SupplierID\\\": 7,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"32 - 500 g boxes\\\",\\r\\n    \\\"Price\\\": 17.45\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 17,\\r\\n    \\\"ProductName\\\": \\\"Alice Mutton\\\",\\r\\n    \\\"SupplierID\\\": 7,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"20 - 1 kg tins\\\",\\r\\n    \\\"Price\\\": 39\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 18,\\r\\n    \\\"ProductName\\\": \\\"Carnarvon Tigers\\\",\\r\\n    \\\"SupplierID\\\": 7,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"16 kg pkg.\\\",\\r\\n    \\\"Price\\\": 62.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 19,\\r\\n    \\\"ProductName\\\": \\\"Teatime Chocolate Biscuits\\\",\\r\\n    \\\"SupplierID\\\": 8,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"10 boxes x 12 pieces\\\",\\r\\n    \\\"Price\\\": 9.2\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 20,\\r\\n    \\\"ProductName\\\": \\\"Sir Rodney's Marmalade\\\",\\r\\n    \\\"SupplierID\\\": 8,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"30 gift boxes\\\",\\r\\n    \\\"Price\\\": 81\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 21,\\r\\n    \\\"ProductName\\\": \\\"Sir Rodney's Scones\\\",\\r\\n    \\\"SupplierID\\\": 8,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"24 pkgs. x 4 pieces\\\",\\r\\n    \\\"Price\\\": 10\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 22,\\r\\n    \\\"ProductName\\\": \\\"Gustaf's Kn\\u00e4ckebr\\u00f6d\\\",\\r\\n    \\\"SupplierID\\\": 9,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"24 - 500 g pkgs.\\\",\\r\\n    \\\"Price\\\": 21\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 23,\\r\\n    \\\"ProductName\\\": \\\"Tunnbr\\u00f6d\\\",\\r\\n    \\\"SupplierID\\\": 9,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"12 - 250 g pkgs.\\\",\\r\\n    \\\"Price\\\": 9\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 24,\\r\\n    \\\"ProductName\\\": \\\"Guaran\\u00e1 Fant\\u00e1stica\\\",\\r\\n    \\\"SupplierID\\\": 10,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"12 - 355 ml cans\\\",\\r\\n    \\\"Price\\\": 4.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 25,\\r\\n    \\\"ProductName\\\": \\\"NuNuCa Nu\\u00df-Nougat-Creme\\\",\\r\\n    \\\"SupplierID\\\": 11,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"20 - 450 g glasses\\\",\\r\\n    \\\"Price\\\": 14\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 26,\\r\\n    \\\"ProductName\\\": \\\"Gumb\\u00e4r Gummib\\u00e4rchen\\\",\\r\\n    \\\"SupplierID\\\": 11,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"100 - 250 g bags\\\",\\r\\n    \\\"Price\\\": 31.23\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 27,\\r\\n    \\\"ProductName\\\": \\\"Schoggi Schokolade\\\",\\r\\n    \\\"SupplierID\\\": 11,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"100 - 100 g pieces\\\",\\r\\n    \\\"Price\\\": 43.9\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 28,\\r\\n    \\\"ProductName\\\": \\\"R\\u00f6ssle Sauerkraut\\\",\\r\\n    \\\"SupplierID\\\": 12,\\r\\n    \\\"CategoryID\\\": 7,\\r\\n    \\\"Unit\\\": \\\"25 - 825 g cans\\\",\\r\\n    \\\"Price\\\": 45.6\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 29,\\r\\n    \\\"ProductName\\\": \\\"Th\\u00fcringer Rostbratwurst\\\",\\r\\n    \\\"SupplierID\\\": 12,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"50 bags x 30 sausgs.\\\",\\r\\n    \\\"Price\\\": 123.79\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 30,\\r\\n    \\\"ProductName\\\": \\\"Nord-Ost Matjeshering\\\",\\r\\n    \\\"SupplierID\\\": 13,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"10 - 200 g glasses\\\",\\r\\n    \\\"Price\\\": 25.89\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 31,\\r\\n    \\\"ProductName\\\": \\\"Gorgonzola Telino\\\",\\r\\n    \\\"SupplierID\\\": 14,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"12 - 100 g pkgs\\\",\\r\\n    \\\"Price\\\": 12.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 32,\\r\\n    \\\"ProductName\\\": \\\"Mascarpone Fabioli\\\",\\r\\n    \\\"SupplierID\\\": 14,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"24 - 200 g pkgs.\\\",\\r\\n    \\\"Price\\\": 32\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 33,\\r\\n    \\\"ProductName\\\": \\\"Geitost\\\",\\r\\n    \\\"SupplierID\\\": 15,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"500 g\\\",\\r\\n    \\\"Price\\\": 2.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 34,\\r\\n    \\\"ProductName\\\": \\\"Sasquatch Ale\\\",\\r\\n    \\\"SupplierID\\\": 16,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 12 oz bottles\\\",\\r\\n    \\\"Price\\\": 14\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 35,\\r\\n    \\\"ProductName\\\": \\\"Steeleye Stout\\\",\\r\\n    \\\"SupplierID\\\": 16,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 12 oz bottles\\\",\\r\\n    \\\"Price\\\": 18\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 36,\\r\\n    \\\"ProductName\\\": \\\"Inlagd Sill\\\",\\r\\n    \\\"SupplierID\\\": 17,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"24 - 250 g jars\\\",\\r\\n    \\\"Price\\\": 19\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 37,\\r\\n    \\\"ProductName\\\": \\\"Gravad lax\\\",\\r\\n    \\\"SupplierID\\\": 17,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"12 - 500 g pkgs.\\\",\\r\\n    \\\"Price\\\": 26\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 38,\\r\\n    \\\"ProductName\\\": \\\"C\\u00f4te de Blaye\\\",\\r\\n    \\\"SupplierID\\\": 18,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"12 - 75 cl bottles\\\",\\r\\n    \\\"Price\\\": 263.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 39,\\r\\n    \\\"ProductName\\\": \\\"Chartreuse verte\\\",\\r\\n    \\\"SupplierID\\\": 18,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"750 cc per bottle\\\",\\r\\n    \\\"Price\\\": 18\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 40,\\r\\n    \\\"ProductName\\\": \\\"Boston Crab Meat\\\",\\r\\n    \\\"SupplierID\\\": 19,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"24 - 4 oz tins\\\",\\r\\n    \\\"Price\\\": 18.4\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 41,\\r\\n    \\\"ProductName\\\": \\\"Jack's New England Clam Chowder\\\",\\r\\n    \\\"SupplierID\\\": 19,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"12 - 12 oz cans\\\",\\r\\n    \\\"Price\\\": 9.65\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 42,\\r\\n    \\\"ProductName\\\": \\\"Singaporean Hokkien Fried Mee\\\",\\r\\n    \\\"SupplierID\\\": 20,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"32 - 1 kg pkgs.\\\",\\r\\n    \\\"Price\\\": 14\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 43,\\r\\n    \\\"ProductName\\\": \\\"Ipoh Coffee\\\",\\r\\n    \\\"SupplierID\\\": 20,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"16 - 500 g tins\\\",\\r\\n    \\\"Price\\\": 46\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 44,\\r\\n    \\\"ProductName\\\": \\\"Gula Malacca\\\",\\r\\n    \\\"SupplierID\\\": 20,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"20 - 2 kg bags\\\",\\r\\n    \\\"Price\\\": 19.45\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 45,\\r\\n    \\\"ProductName\\\": \\\"R\\u00f8gede sild\\\",\\r\\n    \\\"SupplierID\\\": 21,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"1k pkg.\\\",\\r\\n    \\\"Price\\\": 9.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 46,\\r\\n    \\\"ProductName\\\": \\\"Spegesild\\\",\\r\\n    \\\"SupplierID\\\": 21,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"4 - 450 g glasses\\\",\\r\\n    \\\"Price\\\": 12\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 47,\\r\\n    \\\"ProductName\\\": \\\"Zaanse koeken\\\",\\r\\n    \\\"SupplierID\\\": 22,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"10 - 4 oz boxes\\\",\\r\\n    \\\"Price\\\": 9.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 48,\\r\\n    \\\"ProductName\\\": \\\"Chocolade\\\",\\r\\n    \\\"SupplierID\\\": 22,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"10 pkgs.\\\",\\r\\n    \\\"Price\\\": 12.75\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 49,\\r\\n    \\\"ProductName\\\": \\\"Maxilaku\\\",\\r\\n    \\\"SupplierID\\\": 23,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"24 - 50 g pkgs.\\\",\\r\\n    \\\"Price\\\": 20\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 50,\\r\\n    \\\"ProductName\\\": \\\"Valkoinen suklaa\\\",\\r\\n    \\\"SupplierID\\\": 23,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"12 - 100 g bars\\\",\\r\\n    \\\"Price\\\": 16.25\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 51,\\r\\n    \\\"ProductName\\\": \\\"Manjimup Dried Apples\\\",\\r\\n    \\\"SupplierID\\\": 24,\\r\\n    \\\"CategoryID\\\": 7,\\r\\n    \\\"Unit\\\": \\\"50 - 300 g pkgs.\\\",\\r\\n    \\\"Price\\\": 53\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 52,\\r\\n    \\\"ProductName\\\": \\\"Filo Mix\\\",\\r\\n    \\\"SupplierID\\\": 24,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"16 - 2 kg boxes\\\",\\r\\n    \\\"Price\\\": 7\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 53,\\r\\n    \\\"ProductName\\\": \\\"Perth Pasties\\\",\\r\\n    \\\"SupplierID\\\": 24,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"48 pieces\\\",\\r\\n    \\\"Price\\\": 32.8\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 54,\\r\\n    \\\"ProductName\\\": \\\"Tourti\\u00e8re\\\",\\r\\n    \\\"SupplierID\\\": 25,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"16 pies\\\",\\r\\n    \\\"Price\\\": 7.45\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 55,\\r\\n    \\\"ProductName\\\": \\\"P\\u00e2t\\u00e9 chinois\\\",\\r\\n    \\\"SupplierID\\\": 25,\\r\\n    \\\"CategoryID\\\": 6,\\r\\n    \\\"Unit\\\": \\\"24 boxes x 2 pies\\\",\\r\\n    \\\"Price\\\": 24\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 56,\\r\\n    \\\"ProductName\\\": \\\"Gnocchi di nonna Alice\\\",\\r\\n    \\\"SupplierID\\\": 26,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"24 - 250 g pkgs.\\\",\\r\\n    \\\"Price\\\": 38\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 57,\\r\\n    \\\"ProductName\\\": \\\"Ravioli Angelo\\\",\\r\\n    \\\"SupplierID\\\": 26,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"24 - 250 g pkgs.\\\",\\r\\n    \\\"Price\\\": 19.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 58,\\r\\n    \\\"ProductName\\\": \\\"Escargots de Bourgogne\\\",\\r\\n    \\\"SupplierID\\\": 27,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"24 pieces\\\",\\r\\n    \\\"Price\\\": 13.25\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 59,\\r\\n    \\\"ProductName\\\": \\\"Raclette Courdavault\\\",\\r\\n    \\\"SupplierID\\\": 28,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"5 kg pkg.\\\",\\r\\n    \\\"Price\\\": 55\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 60,\\r\\n    \\\"ProductName\\\": \\\"Camembert Pierrot\\\",\\r\\n    \\\"SupplierID\\\": 28,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"15 - 300 g rounds\\\",\\r\\n    \\\"Price\\\": 34\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 61,\\r\\n    \\\"ProductName\\\": \\\"Sirop d'\\u00e9rable\\\",\\r\\n    \\\"SupplierID\\\": 29,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"24 - 500 ml bottles\\\",\\r\\n    \\\"Price\\\": 28.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 62,\\r\\n    \\\"ProductName\\\": \\\"Tarte au sucre\\\",\\r\\n    \\\"SupplierID\\\": 29,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"48 pies\\\",\\r\\n    \\\"Price\\\": 49.3\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 63,\\r\\n    \\\"ProductName\\\": \\\"Vegie-spread\\\",\\r\\n    \\\"SupplierID\\\": 7,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"15 - 625 g jars\\\",\\r\\n    \\\"Price\\\": 43.9\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 64,\\r\\n    \\\"ProductName\\\": \\\"Wimmers gute Semmelkn\\u00f6del\\\",\\r\\n    \\\"SupplierID\\\": 12,\\r\\n    \\\"CategoryID\\\": 5,\\r\\n    \\\"Unit\\\": \\\"20 bags x 4 pieces\\\",\\r\\n    \\\"Price\\\": 33.25\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 65,\\r\\n    \\\"ProductName\\\": \\\"Louisiana Fiery Hot Pepper Sauce\\\",\\r\\n    \\\"SupplierID\\\": 2,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"32 - 8 oz bottles\\\",\\r\\n    \\\"Price\\\": 21.05\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 66,\\r\\n    \\\"ProductName\\\": \\\"Louisiana Hot Spiced Okra\\\",\\r\\n    \\\"SupplierID\\\": 2,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"24 - 8 oz jars\\\",\\r\\n    \\\"Price\\\": 17\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 67,\\r\\n    \\\"ProductName\\\": \\\"Laughing Lumberjack Lager\\\",\\r\\n    \\\"SupplierID\\\": 16,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 12 oz bottles\\\",\\r\\n    \\\"Price\\\": 14\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 68,\\r\\n    \\\"ProductName\\\": \\\"Scottish Longbreads\\\",\\r\\n    \\\"SupplierID\\\": 8,\\r\\n    \\\"CategoryID\\\": 3,\\r\\n    \\\"Unit\\\": \\\"10 boxes x 8 pieces\\\",\\r\\n    \\\"Price\\\": 12.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 69,\\r\\n    \\\"ProductName\\\": \\\"Gudbrandsdalsost\\\",\\r\\n    \\\"SupplierID\\\": 15,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"10 kg pkg.\\\",\\r\\n    \\\"Price\\\": 36\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 70,\\r\\n    \\\"ProductName\\\": \\\"Outback Lager\\\",\\r\\n    \\\"SupplierID\\\": 7,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 355 ml bottles\\\",\\r\\n    \\\"Price\\\": 15\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 71,\\r\\n    \\\"ProductName\\\": \\\"Fl\\u00f8temysost\\\",\\r\\n    \\\"SupplierID\\\": 15,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"10 - 500 g pkgs.\\\",\\r\\n    \\\"Price\\\": 21.5\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 72,\\r\\n    \\\"ProductName\\\": \\\"Mozzarella di Giovanni\\\",\\r\\n    \\\"SupplierID\\\": 14,\\r\\n    \\\"CategoryID\\\": 4,\\r\\n    \\\"Unit\\\": \\\"24 - 200 g pkgs.\\\",\\r\\n    \\\"Price\\\": 34.8\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 73,\\r\\n    \\\"ProductName\\\": \\\"R\\u00f6d Kaviar\\\",\\r\\n    \\\"SupplierID\\\": 17,\\r\\n    \\\"CategoryID\\\": 8,\\r\\n    \\\"Unit\\\": \\\"24 - 150 g jars\\\",\\r\\n    \\\"Price\\\": 15\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 74,\\r\\n    \\\"ProductName\\\": \\\"Longlife Tofu\\\",\\r\\n    \\\"SupplierID\\\": 4,\\r\\n    \\\"CategoryID\\\": 7,\\r\\n    \\\"Unit\\\": \\\"5 kg pkg.\\\",\\r\\n    \\\"Price\\\": 10\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 75,\\r\\n    \\\"ProductName\\\": \\\"Rh\\u00f6nbr\\u00e4u Klosterbier\\\",\\r\\n    \\\"SupplierID\\\": 12,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"24 - 0.5 l bottles\\\",\\r\\n    \\\"Price\\\": 7.75\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 76,\\r\\n    \\\"ProductName\\\": \\\"Lakkalik\\u00f6\\u00f6ri\\\",\\r\\n    \\\"SupplierID\\\": 23,\\r\\n    \\\"CategoryID\\\": 1,\\r\\n    \\\"Unit\\\": \\\"500 ml\\\",\\r\\n    \\\"Price\\\": 18\\r\\n  },\\r\\n  {\\r\\n    \\\"ProductID\\\": 77,\\r\\n    \\\"ProductName\\\": \\\"Original Frankfurter gr\\u00fcne So\\u00dfe\\\",\\r\\n    \\\"SupplierID\\\": 12,\\r\\n    \\\"CategoryID\\\": 2,\\r\\n    \\\"Unit\\\": \\\"12 boxes\\\",\\r\\n    \\\"Price\\\": 13";
            Assert.That(s_response, Does.Contain(s_foo), "Response message does not contain file content of '" + s_attachmentFile + "'");

            /* ******** */
            /* 5th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.POST, s_httpBinUrl + "post", ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS)
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            o_requestClient.AddRequestParameter("param_1", "Hello World!");
            o_requestClient.AddRequestParameter("param_2", 1234.56d.ToString(System.Globalization.CultureInfo.InvariantCulture));
            o_requestClient.AddAttachement("file1", s_attachmentFile);

            s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            Assert.That(s_response, Does.Contain("\"param_1\": \"Hello World!\""), "Response message does not contain '\"param_1\": \"Hello World!\"'");
            Assert.That(s_response, Does.Contain("\"param_2\": \"1234.56\""), "Response message does not contain '\"param_2\": \"1234.56\"'");
            Assert.That(s_response, Does.Contain(s_foo), "Response message does not contain file content of '" + s_attachmentFile + "'");

            /* ******** */
            /* 6th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "basic-auth/user/password")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials,

                AuthenticationUser = "user",
                AuthenticationPassword = "password"
            };

            s_response = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");
            Assert.That(s_response, Does.Contain("\"authenticated\": true"), "Response message does not contain '\"authenticated\": true'");
            Assert.That(s_response, Does.Contain("\"user\": \"user\""), "Response message does not contain '\"user\": \"user\"'");

            /* ******** */
            /* 7th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "basic-auth/user/password")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials,

                AuthenticationUser = "false",
                AuthenticationPassword = "wrong"
            };

            _ = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(401), "Response code is not '401', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("UNAUTHORIZED"), "Response code is not 'UNAUTHORIZED', it is '" + o_requestClient.ResponseMessage + "'");

            /* ******** */
            /* 8th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.DOWNLOAD, s_httpBinUrl + "bytes/1024")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials,

                DownloadFilename = s_testDirectory + "random.txt"
            };

            _ = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(200), "Response code is not '200', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("OK"), "Response code is not 'OK', it is '" + o_requestClient.ResponseMessage + "'");

            Assert.That(ForestNET.Lib.IO.File.Exists(s_testDirectory + "random.txt"), Is.True, "file[" + s_testDirectory + "random.txt" + "] does not exist");
            Assert.That(ForestNET.Lib.IO.File.FileLength(s_testDirectory + "random.txt"), Is.EqualTo(1024), "file length of downloaded file != 1024, file length = " + ForestNET.Lib.IO.File.FileLength(s_testDirectory + "random.txt"));

            /* ******** */
            /* 9th test */
            /* ******** */

            o_requestClient = new(ForestNET.Lib.Net.Request.RequestType.GET, s_httpBinUrl + "url-does-not-exists")
            {
                UseLog = p_b_useLog,
                ProxyAddress = s_proxyAddress,
                ProxyPort = i_proxyPort,
                ProxyUseDefaultCredentials = p_b_useDefaultCredentials
            };

            _ = o_requestClient.ExecuteWebRequest().Result;

            Assert.That(o_requestClient.ResponseCode, Is.EqualTo(404), "Response code is not '404', it is '" + o_requestClient.ResponseCode + "'");
            Assert.That(o_requestClient.ResponseMessage, Is.EqualTo("NOT FOUND"), "Response code is not 'NOT FOUND', it is '" + o_requestClient.ResponseMessage + "'");

            ForestNET.Lib.IO.File.DeleteDirectory(s_testDirectory);
            Assert.That(
                ForestNET.Lib.IO.File.FolderExists(s_testDirectory),
                Is.False,
                "directory[" + s_testDirectory + "] does exist"
            );
        }
    }
}
