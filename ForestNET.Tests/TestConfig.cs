namespace ForestNET.Tests
{
    public class TestConfig
    {
        public static void InitiateTestLogging()
        {
            string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNET.Lib.IO.File.DIR;

            ForestNET.Lib.Global.Instance.InternalLogControl =
            //(byte)ForestNET.Lib.Log.Level.SEVERE;
            (byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING;
            //(byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING + (byte)ForestNET.Lib.Log.Level.INFO;
            //(byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING + (byte)ForestNET.Lib.Log.Level.INFO + (byte)ForestNET.Lib.Log.Level.CONFIG;
            //(byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING + (byte)ForestNET.Lib.Log.Level.INFO + (byte)ForestNET.Lib.Log.Level.CONFIG + (byte)ForestNET.Lib.Log.Level.FINE;
            //(byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING + (byte)ForestNET.Lib.Log.Level.INFO + (byte)ForestNET.Lib.Log.Level.CONFIG + (byte)ForestNET.Lib.Log.Level.FINE + (byte)ForestNET.Lib.Log.Level.FINER;
            //(byte)ForestNET.Lib.Log.Level.SEVERE + (byte)ForestNET.Lib.Log.Level.WARNING + (byte)ForestNET.Lib.Log.Level.INFO + (byte)ForestNET.Lib.Log.Level.CONFIG + (byte)ForestNET.Lib.Log.Level.FINE + (byte)ForestNET.Lib.Log.Level.FINER + (byte)ForestNET.Lib.Log.Level.FINEST;
            //(byte)ForestNET.Lib.Log.Level.ALMOST_ALL;
            //(byte)ForestNET.Lib.Log.Level.ALL;
            ForestNET.Lib.IO.File o_logFile = new(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "test.log", !ForestNET.Lib.IO.File.Exists(s_currentDirectory + ForestNET.Lib.IO.File.DIR + "test.log"));
            o_logFile.TruncateContent();
            List<string> a_logLines =
            [
                "UseFileLogging = true",
                "UseConsoleLogging = false",
                "MinimumLevel = INFO",
                "FileLoggerConfigurationElement = Log,InternalLog;%LocalState;test.log"
            ];
            ForestNET.Lib.Global.Instance.ResetLogFromLines(a_logLines);
        }
    }
}
