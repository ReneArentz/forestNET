namespace ForestNETTests
{
    public class TestConfig
    {
        public static void InitiateTestLogging()
        {
            string s_currentDirectory = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new NullReferenceException("Current directory could not be resolved with '" + System.Reflection.Assembly.GetExecutingAssembly().Location + "'")) + ForestNETLib.IO.File.DIR;

            ForestNETLib.Core.Global.Instance.InternalLogControl =
            //(byte)ForestNETLib.Log.Level.SEVERE;
            (byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING;
            //(byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING + (byte)ForestNETLib.Log.Level.INFO;
            //(byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING + (byte)ForestNETLib.Log.Level.INFO + (byte)ForestNETLib.Log.Level.CONFIG;
            //(byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING + (byte)ForestNETLib.Log.Level.INFO + (byte)ForestNETLib.Log.Level.CONFIG + (byte)ForestNETLib.Log.Level.FINE;
            //(byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING + (byte)ForestNETLib.Log.Level.INFO + (byte)ForestNETLib.Log.Level.CONFIG + (byte)ForestNETLib.Log.Level.FINE + (byte)ForestNETLib.Log.Level.FINER;
            //(byte)ForestNETLib.Log.Level.SEVERE + (byte)ForestNETLib.Log.Level.WARNING + (byte)ForestNETLib.Log.Level.INFO + (byte)ForestNETLib.Log.Level.CONFIG + (byte)ForestNETLib.Log.Level.FINE + (byte)ForestNETLib.Log.Level.FINER + (byte)ForestNETLib.Log.Level.FINEST;
            //(byte)ForestNETLib.Log.Level.ALMOST_ALL;
            //(byte)ForestNETLib.Log.Level.ALL;
            ForestNETLib.IO.File o_logFile = new(s_currentDirectory + ForestNETLib.IO.File.DIR + "test.log", !ForestNETLib.IO.File.Exists(s_currentDirectory + ForestNETLib.IO.File.DIR + "test.log"));
            o_logFile.TruncateContent();
            List<string> a_logLines =
            [
                "UseFileLogging = true",
                "UseConsoleLogging = false",
                "MinimumLevel = INFO",
                "FileLoggerConfigurationElement = Log,InternalLog;%LocalState;test.log"
            ];
            ForestNETLib.Core.Global.Instance.ResetLogFromLines(a_logLines);
        }
    }
}
