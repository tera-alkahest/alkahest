using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Packager
{
    sealed class LocalPackage
    {
        public string Name { get; }

        public string Path { get; }

        public string ManifestFilePath { get; }

        public Version Version { get; }

        public IReadOnlyList<string> Files { get; }

        public IReadOnlyList<string> Dependencies { get; }

        public LocalPackage(string name)
        {
            Name = name;
            Path = Package.GetPath(name);
            ManifestFilePath = GetManifestFilePath();

            using var reader = new JsonTextReader(new StreamReader(ManifestFilePath));
            var stamp = JObject.Load(reader);

            Version = Version.Parse((string)stamp["version"]);
            Files = stamp["files"].Select(x => (string)x).ToList();
            Dependencies = stamp["dependencies"].Select(x => (string)x).ToList();
        }

        public LocalPackage(Package package)
        {
            Name = package.Name;
            Path = package.Path;
            ManifestFilePath = GetManifestFilePath();
            Version = package.Version;
            Files = package.Files;
            Dependencies = package.Dependencies;

            using var writer = new JsonTextWriter(new StreamWriter(ManifestFilePath))
            {
                Formatting = Formatting.Indented,
            };

            new JObject()
            {
                ["version"] = new JValue(Version.ToString()),
                ["files"] = new JArray(Files.Select(x => new JValue(x)).ToArray()),
                ["dependencies"] = new JArray(Dependencies.Select(x => new JValue(x)).ToArray()),
            }.WriteTo(writer);
        }

        string GetManifestFilePath()
        {
            return System.IO.Path.Combine(Path, PackageManager.ManifestFileName);
        }
    }
}
