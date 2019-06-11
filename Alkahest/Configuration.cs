using Alkahest.Core;
using Alkahest.Core.Logging;
using System;
using System.Configuration;
using System.Linq;
using System.Net;

namespace Alkahest
{
    static class Configuration
    {
        public static LogLevel LogLevel { get; }

        public static string LogTimestampFormat { get; }

        public static string[] DiscardLogSources { get; }

        public static string[] Loggers { get; }

        public static bool ColorsEnabled { get; }

        public static ConsoleColor ErrorColor { get; }

        public static ConsoleColor WarningColor { get; }

        public static ConsoleColor BasicColor { get; }

        public static ConsoleColor InfoColor { get; }

        public static ConsoleColor DebugColor { get; }

        public static string LogDirectory { get; }

        public static string LogFileNameFormat { get; }

        public static string PluginDirectory { get; }

        public static string PluginPattern { get; }

        public static string[] DisablePlugins { get; }

        public static Uri PackageRegistryUri { get; }

        public static string PackageDirectory { get; }

        public static Uri AssetManifestUri { get; }

        public static string AssetDirectory { get; }

        public static TimeSpan AssetTimeout { get; }

        public static string UpgradeDirectory { get; }

        public static string UpgradeOwner { get; }

        public static string UpgradeRepository { get; }

        public static bool DataCenterInterning { get; }

        public static Region Region { get; }

        public static bool ServerListEnabled { get; }

        public static TimeSpan ServerListTimeout { get; }

        public static int ServerListRetries { get; }

        public static IPAddress ServerListBaseAddress { get; }

        public static IPAddress GameBaseAddress { get; }

        public static int ServerListPort { get; }

        public static int GameBasePort { get; }

        public static int GameBacklog { get; }

        public static int GameMaxClients { get; }

        public static TimeSpan GameTimeout { get; }

        public static int PoolLimit { get; }

        public static bool AdjustHostsFile { get; }

        public static bool AdjustCertificateStore { get; }

        static Configuration()
        {
            var cfg = ConfigurationManager.AppSettings;

            LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), cfg["logLevel"], true);
            LogTimestampFormat = cfg["logTimestampFormat"];
            DiscardLogSources = Split(cfg["discardLogSources"], ',');
            Loggers = Split(cfg["loggers"], ',');
            ColorsEnabled = bool.Parse(cfg["enableColors"]);
            ErrorColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["errorColor"], true);
            WarningColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["warningColor"], true);
            BasicColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["basicColor"], true);
            InfoColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["infoColor"], true);
            DebugColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["debugColor"], true);
            LogDirectory = cfg["logDirectory"];
            LogFileNameFormat = cfg["logFileNameFormat"];
            PluginDirectory = cfg["pluginDirectory"];
            PluginPattern = cfg["pluginPattern"];
            DisablePlugins = Split(cfg["disablePlugins"], ',');
            PackageRegistryUri = new Uri(cfg["packageRegistryUri"]);
            PackageDirectory = cfg["packageDirectory"];
            AssetManifestUri = new Uri(cfg["assetManifestUri"]);
            AssetDirectory = cfg["assetDirectory"];
            AssetTimeout = TimeSpan.FromMinutes(int.Parse(cfg["assetTimeout"]));
            UpgradeDirectory = cfg["upgradeDirectory"];
            UpgradeOwner = cfg["upgradeOwner"];
            UpgradeRepository = cfg["upgradeRepository"];
            DataCenterInterning = bool.Parse(cfg["dataCenterInterning"]);
            Region = (Region)Enum.Parse(typeof(Region), cfg["region"], true);
            ServerListEnabled = bool.Parse(cfg["enableSls"]);
            ServerListBaseAddress = IPAddress.Parse(cfg["slsBaseAddress"]);
            GameBaseAddress = IPAddress.Parse(cfg["gameBaseAddress"]);
            ServerListPort = int.Parse(cfg["slsPort"]);
            GameBasePort = int.Parse(cfg["gameBasePort"]);
            ServerListTimeout = TimeSpan.FromSeconds(int.Parse(cfg["slsTimeout"]));
            ServerListRetries = int.Parse(cfg["slsRetries"]);
            GameBacklog = int.Parse(cfg["gameBacklog"]);
            GameMaxClients = int.Parse(cfg["gameMaxClients"]);
            GameTimeout = TimeSpan.FromMinutes(int.Parse(cfg["gameTimeout"]));
            PoolLimit = int.Parse(cfg["poolLimit"]);
            AdjustHostsFile = bool.Parse(cfg["adjustHostsFile"]);
            AdjustCertificateStore = bool.Parse(cfg["adjustCertificateStore"]);
        }

        static string[] Split(string value, char separator)
        {
            return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();
        }
    }
}
