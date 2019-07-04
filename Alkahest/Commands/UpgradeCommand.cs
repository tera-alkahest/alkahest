using Alkahest.Core.Logging;
using Alkahest.Core.Reflection;
using Alkahest.Packager;
using Mono.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alkahest.Commands
{
    sealed class UpgradeCommand : AlkahestCommand
    {
        const string UpgraderName = "Alkahest.Upgrader.exe";

        static readonly Log _log = new Log(typeof(UpgradeCommand));

        public UpgradeCommand()
            : base("Packager", "upgrade", "Upgrade to the latest Alkahest version")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name}",
            };
        }

        protected override int Invoke(string[] args)
        {
            _log.Info("Checking for a newer release...");

            var current = Assembly.GetExecutingAssembly().GetInformationalVersion(true);
            var latest = GitHub.Client.Repository.Release.GetAll(Configuration.UpgradeOwner,
                Configuration.UpgradeRepository).Result.OrderByDescending(x => x.PublishedAt).First();

            if (current == latest.TagName)
            {
                _log.Basic("No new release found");
                return 0;
            }

            _log.Basic("Upgrading from {0} to {1}...", current, latest.TagName);

            var asset = latest.Assets.Single(x => Path.GetExtension(x.Name) == ".zip");

            _log.Info("Downloading {0}...", asset.Name);

            var data = GitHub.GetBytes(new Uri(asset.BrowserDownloadUrl));
            var dir = Configuration.UpgradeDirectory;
            var zip = Path.Combine(dir, asset.Name);

            Directory.CreateDirectory(dir);
            File.WriteAllBytes(zip, data);

            _log.Info("Extracting {0}...", zip);

            var exe = Path.Combine(dir, UpgraderName);

            File.Copy(UpgraderName, exe, true);

            var dest = Path.GetFullPath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            using var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = $"{Process.GetCurrentProcess().Id} \"{asset.Name}\" \"{dest}\"",
                    FileName = exe,
                    UseShellExecute = false,
                    WorkingDirectory = dir,
                },
            };

            proc.Start();

            return 0;
        }
    }
}
