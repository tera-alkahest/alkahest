using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Packager
{
    sealed class PackageManager
    {
        public const string PackageFileName = "package.json";

        public const string ManifestFileName = "manifest.json";

        static readonly Log _log = new Log(typeof(PackageManager));

        public IReadOnlyDictionary<string, Package> Registry { get; }

        readonly Dictionary<string, LocalPackage> _locals = new Dictionary<string, LocalPackage>();

        public IReadOnlyDictionary<string, LocalPackage> LocalPackages => _locals;

        public PackageManager()
        {
            var pkgs = Configuration.PackageDirectory;

            Directory.CreateDirectory(pkgs);

            _log.Info("Fetching package registry...");

            var registry = JArray.Parse(GitHub.GetString(Configuration.PackageRegistryUri))
                .Select(x => new Package((JObject)x)).ToDictionary(x => x.Name);

            foreach (var pkg in Registry.Values.ToArray())
            {
                var abs = Path.GetFullPath(pkg.Path);
                var bad = false;

                void CheckReferredPackages(bool dependencies)
                {
                    foreach (var name in dependencies ? pkg.Dependencies : pkg.Conflicts)
                    {
                        if (!registry.ContainsKey(name))
                        {
                            _log.Warning("Package {0} has {1} package {2} which does not exist; ignoring package",
                                pkg.Name, dependencies ? "dependency" : "conflicting", name);
                            bad = true;
                        }
                    }
                }

                CheckReferredPackages(true);
                CheckReferredPackages(false);

                foreach (var file in pkg.Files)
                {
                    var loc = Path.GetFullPath(Path.Combine(abs, file));

                    if (!loc.StartsWith(abs + Path.DirectorySeparatorChar) &&
                        !loc.StartsWith(abs + Path.AltDirectorySeparatorChar))
                    {
                        _log.Warning("Package {0} has file path {1} which is illegal; ignoring package",
                            pkg.Name, file);
                        bad = true;
                    }

                    if (loc == Path.Combine(abs, PackageFileName) ||
                        loc == Path.Combine(abs, ManifestFileName))
                    {
                        _log.Warning("Package {0} has file path {1} which is for internal use; ignoring package",
                            pkg.Name, file);
                        bad = true;
                    }
                }

                if (bad)
                    registry.Remove(pkg.Name);
            }

            Registry = registry;

            _log.Info("Reading local package manifests...");

            foreach (var dir in Directory.EnumerateDirectories(pkgs))
            {
                if (!File.Exists(Path.Combine(dir, ManifestFileName)))
                    continue;

                var pkg = new LocalPackage(Path.GetFileName(dir));

                _locals.Add(pkg.Name, pkg);
            }
        }

        public void Install(Package package)
        {
            Directory.CreateDirectory(package.Path);

            foreach (var file in package.Files)
            {
                var obj = GitHub.GetObject(new Uri(
                    $"https://raw.githubusercontent.com/{package.Owner}/{package.Repository}/master/{file}"));
                var path = Path.Combine(package.Path, file);

                switch (obj)
                {
                    case string s:
                        File.WriteAllText(path, s);
                        break;
                    case byte[] b:
                        File.WriteAllBytes(path, b);
                        break;
                }

                _log.Info("Added {0}", path);
            }

            _locals.Add(package.Name, new LocalPackage(package));
        }

        public void Uninstall(LocalPackage package, bool everything)
        {
            File.Delete(package.ManifestFilePath);

            var dirs = new HashSet<string>();

            foreach (var file in package.Files)
            {
                var path = Path.Combine(package.Path, file);

                File.Delete(path);

                _log.Info("Removed {0}", path);

                var dir = Path.GetDirectoryName(path);

                if (dir != string.Empty)
                    dirs.Add(dir);
            }

            DirectoryInfo info;

            foreach (var dir in dirs)
                if ((info = new DirectoryInfo(dir)).Exists && !info.EnumerateFileSystemInfos().Any())
                    Directory.Delete(dir, true);

            _locals.Remove(package.Name);

            if (everything)
                Directory.Delete(package.Path, true);
        }

        public void Update(LocalPackage local, Package latest)
        {
            Uninstall(local, false);
            Install(latest);
        }
    }
}
