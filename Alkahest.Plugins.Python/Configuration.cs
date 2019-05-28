using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Alkahest.Plugins.Python
{
    static class Configuration
    {
        public static string PackageDirectory { get; }

        public static string StdLibDirectory { get; }

        public static string[] DisablePackages { get; }

        static Configuration()
        {
            var cfg = ToNameValueCollection(ConfigurationManager.OpenExeConfiguration(
                Assembly.GetExecutingAssembly().Location).AppSettings.Settings);

            PackageDirectory = cfg["packageDirectory"];
            StdLibDirectory = cfg["stdLibDirectory"];
            DisablePackages = Split(cfg["disablePackages"], ',');
        }

        static NameValueCollection ToNameValueCollection(KeyValueConfigurationCollection collection)
        {
            var col = new NameValueCollection();

            foreach (KeyValueConfigurationElement elem in collection)
                col.Add(elem.Key, elem.Value);

            return col;
        }

        static string[] Split(string value, char separator)
        {
            return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();
        }
    }
}
