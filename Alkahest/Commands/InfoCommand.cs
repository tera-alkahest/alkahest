using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System;
using System.Linq;

namespace Alkahest.Commands
{
    sealed class InfoCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(InfoCommand));

        public InfoCommand()
            : base("Packager", "info", "Show information about the given packages")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} PACKAGE...",
            };
        }

        protected override int Invoke(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Expected at least 1 argument");
                return 1;
            }

            var mgr = new PackageManager();
            var valid = false;

            foreach (var name in args)
            {
                mgr.Registry.TryGetValue(name, out var latest);

                if (latest == null)
                {
                    if (valid)
                        _log.Basic(string.Empty);

                    _log.Error("Package {0} does not exist", name);
                    continue;
                }

                valid = true;

                mgr.LocalPackages.TryGetValue(name, out var local);

                _log.Basic(string.Empty);
                _log.Basic("  {0} | {1} | https://github.com/{2}/{3}", latest.Name, latest.License,
                    latest.Owner, latest.Repository);
                _log.Basic("    Latest version: {0}", latest.Version);

                if (local != null)
                {
                    const string format = "    Installed version: {0}";

                    if (local.Version < latest.Version)
                        _log.Warning(format, local.Version);
                    else
                        _log.Basic(format, local.Version);
                }

                _log.Basic("    Contributors:");

                foreach (var user in latest.Contributors)
                    _log.Basic("      https://github.com/{0}", user);

                _log.Basic("    Files:");

                foreach (var file in latest.Files)
                    _log.Basic("      https://github.com/{0}/{1}/blob/master/{2}", latest.Owner,
                        latest.Repository, file);

                if (latest.Conflicts.Count != 0)
                {
                    _log.Basic("    Conflicts:");

                    foreach (var cname in latest.Conflicts)
                    {
                        const string format = "      {0}{1}";

                        if (mgr.LocalPackages.ContainsKey(cname))
                            _log.Error(format, cname, " (installed)");
                        else
                            _log.Basic(format, cname, string.Empty);
                    }
                }

                foreach (var line in latest.Description.Split('\n').Select(x => x.Trim()))
                {
                    _log.Basic(string.Empty);
                    _log.Basic("    {0}", line);
                }
            }

            return 0;
        }
    }
}
