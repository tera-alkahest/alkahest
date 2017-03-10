using System;
using System.Configuration;
using System.Linq;
using System.Net;
using Alkahest.Core;
using Alkahest.Core.Logging;

namespace Alkahest.Server
{
    static class Configuration
    {
        public static LogLevel LogLevel { get; }

        public static string LogTimestampFormat { get; }

        public static string[] DiscardLogSources { get; }

        public static string[] Loggers { get; }

        public static bool EnableColors { get; }

        public static ConsoleColor ErrorColor { get; }

        public static ConsoleColor BasicColor { get; }

        public static ConsoleColor InfoColor { get; }

        public static ConsoleColor DebugColor { get; }

        public static string LogDirectory { get; }

        public static string LogFileNameFormat { get; }

        public static bool EnablePacketLogs { get; }

        public static string PacketLogDirectory { get; }

        public static string PacketLogFileNameFormat { get; }

        public static bool CompressPacketLogs { get; }

        public static string PluginDirectory { get; }

        public static string PluginPattern { get; }

        public static string[] DisablePlugins { get; }

        public static Region Region { get; }

        public static TimeSpan ServerListTimeout { get; }

        public static int ServerListRetries { get; }

        public static IPAddress ServerListAddress { get; }

        public static IPAddress GameAddress { get; }

        public static int GameBacklog { get; }

        public static int GameMaxClients { get; }

        public static TimeSpan GameTimeout { get; }

        public static int GamePacketRoundtrips { get; }

        public static int PoolLimit { get; }

        public static bool AdjustHostsFile { get; }

        public static bool EnableAssertions { get; }

        static Configuration()
        {
            var cfg = ConfigurationManager.AppSettings;

            LogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), cfg["logLevel"], true);
            LogTimestampFormat = cfg["logTimestampFormat"];
            DiscardLogSources = Split(cfg["discardLogSources"], ',');
            Loggers = Split(cfg["loggers"], ',');
            EnableColors = bool.Parse(cfg["enableColors"]);
            ErrorColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["errorColor"], true);
            BasicColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["basicColor"], true);
            InfoColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["infoColor"], true);
            DebugColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), cfg["debugColor"], true);
            LogDirectory = cfg["logDirectory"];
            LogFileNameFormat = cfg["logFileNameFormat"];
            EnablePacketLogs = bool.Parse(cfg["enablePacketLogs"]);
            PacketLogDirectory = cfg["packetLogDirectory"];
            PacketLogFileNameFormat = cfg["packetLogFileNameFormat"];
            CompressPacketLogs = bool.Parse(cfg["compressPacketLogs"]);
            PluginDirectory = cfg["pluginDirectory"];
            PluginPattern = cfg["pluginPattern"];
            DisablePlugins = Split(cfg["disablePlugins"], ',');
            Region = (Region)Enum.Parse(typeof(Region), cfg["region"], true);
            ServerListAddress = IPAddress.Parse(cfg["slsAddress"]);
            GameAddress = IPAddress.Parse(cfg["gameAddress"]);
            ServerListTimeout = TimeSpan.FromSeconds(int.Parse(cfg["slsTimeout"]));
            ServerListRetries = int.Parse(cfg["slsRetries"]);
            GameBacklog = int.Parse(cfg["gameBacklog"]);
            GameMaxClients = int.Parse(cfg["gameMaxClients"]);
            GameTimeout = TimeSpan.FromMinutes(int.Parse(cfg["gameTimeout"]));
            GamePacketRoundtrips = int.Parse(cfg["gamePacketRoundtrips"]);
            PoolLimit = int.Parse(cfg["poolLimit"]);
            AdjustHostsFile = bool.Parse(cfg["adjustHostsFile"]);
            EnableAssertions = bool.Parse(cfg["enableAssertions"]);
        }

        static string[] Split(string value, char separator)
        {
            return value.Split(new[] { separator },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()).ToArray();
        }
    }
}
