using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Commands
{
    sealed class UninstallCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(UninstallCommand));

        bool _force;

        bool _everything;

        public UninstallCommand()
            : base("Packager", "uninstall", "Uninstall packages specified by name")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [PACKAGE...] [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "f|force",
                    $"Enable/disable forcing package removals even if dependent packages would break (defaults to `{_force}`)",
                    f => _force = f != null
                },
                {
                    "e|everything",
                    $"Enable/disable removing everything in specified package directories (defaults to `{_everything}`)",
                    e => _everything = e != null
                },
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
            var pkgs = new HashSet<LocalPackage>();

            foreach (var name in args)
            {
                if (!mgr.LocalPackages.TryGetValue(name, out var pkg))
                {
                    if (mgr.Registry.ContainsKey(name))
                        _log.Error("Package {0} is not installed", name);
                    else
                        _log.Error("Package {0} does not exist", name);

                    return 1;
                }

                pkgs.Add(pkg);
            }

            var installed = mgr.LocalPackages.Values.ToHashSet();

            installed.ExceptWith(pkgs);

            foreach (var pkg in installed)
            {
                foreach (var name in pkg.Dependencies)
                {
                    if (!pkgs.Any(x => x.Name == name))
                        continue;

                    const string format = "Package {0} depends on to-be-uninstalled package {1}{2}";

                    if (!_force)
                    {
                        _log.Error(format, pkg.Name, name, string.Empty);
                        return 1;
                    }

                    _log.Warning(format, pkg.Name, name, "; ignored");
                }
            }

            _log.Basic("Packages to be uninstalled:");
            _log.Basic(string.Empty);

            foreach (var pkg in pkgs)
                _log.Basic("  {0}", pkg.Name);

            _log.Basic(string.Empty);

            var count = 0;

            foreach (var pkg in pkgs)
            {
                _log.Info("Uninstalling package {0}...", pkg.Name);

                mgr.Uninstall(pkg, _everything);

                count++;
            }

            _log.Basic("Uninstalled {0} packages", count);

            return 0;
        }
    }
}
