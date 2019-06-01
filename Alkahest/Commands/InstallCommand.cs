using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Commands
{
    sealed class InstallCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(InstallCommand));

        bool _force;

        public InstallCommand()
            : base("Packager", "install", "Install packages specified by name")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [PACKAGE...] [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "f|force",
                    $"Enable/disable forcing package installations even if conflicts are detected (defaults to `{_force}`)",
                    f => _force = f != null
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
            var installing = new HashSet<Package>();

            foreach (var name in args)
            {
                if (!mgr.Registry.TryGetValue(name, out var pkg))
                {
                    _log.Error("Package {0} does not exist", name);
                    return 1;
                }

                installing.Add(pkg);
            }

            void AddDependencies(Package package)
            {
                foreach (var name in package.Dependencies)
                    if (mgr.Registry.TryGetValue(name, out var dep) && installing.Add(dep))
                        AddDependencies(dep);
            }

            foreach (var pkg in installing.ToArray())
                AddDependencies(pkg);

            _log.Basic("Packages to be installed:");
            _log.Basic(string.Empty);

            foreach (var pkg in installing.ToArray())
            {
                var installed = mgr.LocalPackages.ContainsKey(pkg.Name);

                if (installed)
                    installing.Remove(pkg);

                var status = installed ? " (already installed; skipping)" : string.Empty;

                _log.Basic("  {0}{1}", pkg.Name, status);
            }

            _log.Basic(string.Empty);

            foreach (var pkg in installing)
            {
                var finfo = new FileInfo(pkg.Path);
                var dinfo = new DirectoryInfo(pkg.Path);

                if (!finfo.Exists && !dinfo.Exists)
                    continue;

                _log.Error("Package path {0} exists already", pkg.Path);
                return 1;
            }

            foreach (var pkg in installing)
            {
                foreach (var name in pkg.Conflicts)
                {
                    if (mgr.LocalPackages.ContainsKey(name))
                    {
                        const string format = "Package {0} conflicts with already-installed package {1}{2}";

                        if (!_force)
                        {
                            _log.Error(format, pkg.Name, name, string.Empty);
                            return 1;
                        }

                        _log.Warning(format, pkg.Name, name, "; ignoring");
                    }

                    if (installing.Any(x => x.Name == name))
                    {
                        const string format = "Package {0} conflicts with to-be-installed package {1}{2}";

                        if (!_force)
                        {
                            _log.Error(format, pkg.Name, name, string.Empty);
                            return 1;
                        }

                        _log.Warning(format, pkg.Name, name, "; ignoring");
                    }
                }
            }

            var count = 0;

            foreach (var pkg in installing)
            {
                _log.Info("Installing package {0}...", pkg.Name);

                mgr.Install(pkg);

                count++;
            }

            _log.Basic("Installed {0} packages", count);

            return 0;
        }
    }
}
