using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;

namespace Alkahest.Plugins.PacketLogger
{
    static class Configuration
    {
        public static string LogDirectory { get; }

        public static string LogFileNameFormat { get; }

        public static bool CompressLogs { get; }

        static Configuration()
        {
            var cfg = ToNameValueCollection(ConfigurationManager.OpenExeConfiguration(
                Assembly.GetExecutingAssembly().Location).AppSettings.Settings);

            LogDirectory = cfg["logDirectory"];
            LogFileNameFormat = cfg["logFileNameFormat"];
            CompressLogs = bool.Parse(cfg["compressLogs"]);
        }

        static NameValueCollection ToNameValueCollection(KeyValueConfigurationCollection collection)
        {
            var col = new NameValueCollection();

            foreach (KeyValueConfigurationElement elem in collection)
                col.Add(elem.Key, elem.Value);

            return col;
        }
    }
}
