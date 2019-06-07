using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Packager
{
    sealed class Package
    {
        public string Name { get; }

        public string Path { get; }

        public string Description { get; }

        public string License { get; }

        public string Owner { get; }

        public string Repository { get; }

        public Version Version { get; }

        public IReadOnlyList<string> Contributors { get; }

        public IReadOnlyList<string> Files { get; }

        public IReadOnlyList<string> Dependencies { get; }

        public IReadOnlyList<string> Conflicts { get; }

        public IReadOnlyList<AssetKind> Assets { get; }

        public Package(JObject obj)
        {
            Name = (string)obj["name"];
            Path = GetPath(Name);
            Description = (string)obj["description"];
            License = (string)obj["license"];
            Owner = (string)obj["owner"];
            Repository = (string)obj["repository"];

            var details = JObject.Parse(GitHub.GetString(new Uri(
                $"https://raw.githubusercontent.com/{Owner}/{Repository}/master/{PackageManager.PackageFileName}")));

            Version = Version.Parse((string)details["version"]);
            Contributors = details["contributors"].Select(x => (string)x).ToList();
            Files = details["files"].Select(x => (string)x).ToList();
            Dependencies = (details["dependencies"] ?? new JArray()).Select(x => (string)x).ToList();
            Conflicts = (details["conflicts"] ?? new JArray()).Select(x => (string)x).ToList();
            Assets = (details["assets"] ?? new JArray()).Select(
                x => (AssetKind)Enum.Parse(typeof(AssetKind), (string)x)).ToList();
        }

        public static string GetPath(string name)
        {
            return System.IO.Path.Combine(Configuration.PackageDirectory, name);
        }
    }
}
