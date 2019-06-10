using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Theraot.Collections;

namespace Alkahest.Commands
{
    sealed class UpdateCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(UpdateCommand));

        bool _force;

        public UpdateCommand()
            : base("Packager", "update", "Update installed packages and assets")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [PACKAGE...] [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "f|force",
                    $"Enable/disable forcing package updates even if conflicts are detected (defaults to `{_force}`)",
                    f => _force = f != null
                },
            };
        }

        protected override int Invoke(string[] args)
        {
            new AssetManager().UpdateAll();

            var mgr = new PackageManager();
            var pkgs = new HashSet<LocalPackage>();

            if (args.Length != 0)
            {
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
            }
            else
                foreach (var pkg in mgr.LocalPackages.Values)
                    pkgs.Add(pkg);

            var updating = new HashSet<(LocalPackage, Package)>();

            foreach (var pkg in pkgs)
            {
                if (mgr.Registry.TryGetValue(pkg.Name, out var latest))
                {
                    if (pkg.Version < latest.Version)
                        updating.Add((pkg, latest));
                    else
                        _log.Info("Package {0} is up to date", pkg.Name,
                            pkg.Version, latest.Version);
                }
                else
                    _log.Warning("Package {0} does not exist in the registry; skipping", pkg.Name);
            }

            var installing = new HashSet<Package>();

            void AddDependencies(Package package)
            {
                foreach (var name in package.Dependencies)
                    if (mgr.Registry.TryGetValue(name, out var dep) && installing.Add(dep))
                        AddDependencies(dep);
            }

            foreach (var pkg in updating.Select(x => x.Item2))
            {
                installing.Add(pkg);
                AddDependencies(pkg);
            }

            var existing = mgr.Registry.Values.Where(x => mgr.LocalPackages.ContainsKey(x.Name)).ToHashSet();

            installing.ExceptWith(existing);

            if (installing.Any())
            {
                _log.Basic("Package dependencies to be installed:");
                _log.Basic(string.Empty);

                foreach (var pkg in installing)
                    _log.Basic("  {0}", pkg.Name);

                _log.Basic(string.Empty);
            }

            foreach (var pkg in installing)
            {
                var finfo = new FileInfo(pkg.Path);
                var dinfo = new DirectoryInfo(pkg.Path);

                if (!finfo.Exists && !dinfo.Exists)
                    continue;

                _log.Error("Package path {0} exists already", pkg.Path);
                return 1;
            }

            var changing = new HashSet<Package>();

            changing.AddRange(updating.Select(x => x.Item2));
            changing.AddRange(installing);

            foreach (var pkg in changing)
            {
                foreach (var name in pkg.Conflicts)
                {
                    if (updating.Any(x => x.Item2.Name == name))
                    {
                        const string format = "Package {0} conflicts with to-be-updated package {1}{2}";

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

            if (installing.Any())
            {
                foreach (var pkg in installing)
                {
                    _log.Info("Installing package {0}...", pkg.Name);

                    mgr.Install(pkg);

                    count++;
                }

                _log.Basic("Installed {0} dependency packages", count);
            }

            count = 0;

            foreach (var (local, latest) in updating)
            {
                _log.Info("Updating package {0}...", local.Name);

                mgr.Update(local, latest);

                count++;
            }

            _log.Basic("Updated {0} packages", count);

            if (updating.Any(x => x.Item2.Assets.Contains(AssetKind.DataCenter)))
                new AssetManager().UpdateDataCenter();

            return 0;
        }
    }
}
