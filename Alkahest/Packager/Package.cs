using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alkahest.Packager
{
    sealed class Package
    {
        static readonly Log _log = new Log(typeof(Package));

        public PackageKind Kind { get; }

        public string Name { get; }

        public string Path { get; }

        public string Description { get; }

        public string License { get; }

        public string Owner { get; }

        public string Repository { get; }

        Version _version;

        public Version Version
        {
            get
            {
                PopulateDetails();

                return _version;
            }
        }

        IReadOnlyList<string> _contributors;

        public IReadOnlyList<string> Contributors
        {
            get
            {
                PopulateDetails();

                return _contributors;
            }
        }

        IReadOnlyList<string> _files;

        public IReadOnlyList<string> Files
        {
            get
            {
                PopulateDetails();

                return _files;
            }
        }

        IReadOnlyList<string> _dependencies;

        public IReadOnlyList<string> Dependencies
        {
            get
            {
                PopulateDetails();

                return _dependencies;
            }
        }

        IReadOnlyList<string> _conflicts;

        public IReadOnlyList<string> Conflicts
        {
            get
            {
                PopulateDetails();

                return _conflicts;
            }
        }

        public Package(JObject obj)
        {
            Kind = (PackageKind)Enum.Parse(typeof(PackageKind), (string)obj["kind"], true);
            Name = (string)obj["name"];
            Path = GetPath(Kind, Name);
            Description = (string)obj["description"];
            License = (string)obj["license"];
            Owner = (string)obj["owner"];
            Repository = (string)obj["repository"];
        }

        void PopulateDetails()
        {
            if (_version != null)
                return;

            var json = GitHub.GetString(
                new Uri($"https://raw.githubusercontent.com/{Owner}/{Repository}/master/package.json"));
            var details = JObject.Parse(json);
            var version = (string)details["version"];

            if (!Version.TryParse(version, out _version))
            {
                _version = new Version(1, 0, 0);

                _log.Warning("Package {0} has invalid version {1}; assuming {2}", version, _version);
            }

            _contributors = details["contributors"].Select(x => (string)x).ToList();
            _files = details["files"].Select(x => (string)x).ToList();
            _dependencies = (details["dependencies"] ?? new JArray()).Select(x => (string)x).ToList();
            _conflicts = (details["conflicts"] ?? new JArray()).Select(x => (string)x).ToList();
        }

        public static string GetPath(PackageKind kind, string name)
        {
            return System.IO.Path.Combine(kind == PackageKind.CSharp ?
                Configuration.CSharpPackageDirectory : Configuration.PythonPackageDirectory, name);
        }
    }
}
