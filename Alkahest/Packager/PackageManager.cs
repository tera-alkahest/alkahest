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
        public const string ManifestFileName = "manifest.json";

        static readonly Log _log = new Log(typeof(PackageManager));

        public IReadOnlyDictionary<string, Package> Registry { get; }

        readonly Dictionary<string, LocalPackage> _locals = new Dictionary<string, LocalPackage>();

        public IReadOnlyDictionary<string, LocalPackage> LocalPackages => _locals;

        public PackageManager()
        {
            var csharp = Configuration.CSharpPackageDirectory;
            var python = Configuration.PythonPackageDirectory;

            Directory.CreateDirectory(csharp);
            Directory.CreateDirectory(python);

            _log.Info("Fetching package registry...");

            Registry = JArray.Parse(GitHub.GetString(Configuration.PackageRegistryUri))
                .Select(x => new Package((JObject)x)).ToDictionary(x => x.Name);

            void CheckReferredPackages(bool dependencies)
            {
                foreach (var pkg in Registry.Values)
                    foreach (var name in dependencies ? pkg.Dependencies : pkg.Conflicts)
                        if (!Registry.ContainsKey(name))
                            _log.Warning("Package {0} has {1} package {2} which does not exist; ignoring",
                                pkg.Name, dependencies ? "dependency" : "conflicting", name);
            }

            CheckReferredPackages(true);
            CheckReferredPackages(false);

            _log.Info("Reading local package manifests...");

            void AddLocalPackages(PackageKind kind)
            {
                foreach (var dir in Directory.EnumerateDirectories(
                    kind == PackageKind.CSharp ? csharp : python))
                {
                    if (!File.Exists(Path.Combine(dir, ManifestFileName)))
                        continue;

                    var pkg = new LocalPackage(kind, Path.GetFileName(dir));

                    _locals.Add(pkg.Name, pkg);
                }
            }

            AddLocalPackages(PackageKind.CSharp);
            AddLocalPackages(PackageKind.Python);
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
