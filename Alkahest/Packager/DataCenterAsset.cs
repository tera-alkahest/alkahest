using Alkahest.Core;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace Alkahest.Packager
{
    sealed class DataCenterAsset : IAsset
    {
        public FileInfo File { get; }

        readonly Region _region;

        readonly string _hash;

        public DataCenterAsset(string directory, Region region, JObject obj)
        {
            File = new FileInfo(Path.Combine(directory, (string)obj["name"]));
            _region = region;
            _hash = (string)obj["sha1"];
        }

        public bool CheckIfLatest()
        {
            if (!File.Exists)
                return false;

            using var sha = new SHA1Managed();

            return _hash == BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(File.FullName)))
                .Replace("-", string.Empty).ToLowerInvariant();
        }

        public void Update()
        {
            var region = _region.ToString().ToLowerInvariant();
            var zipName = File.FullName + ".zip";

            System.IO.File.WriteAllBytes(zipName, GitHub.GetBytes(new Uri(
                $"https://github.com/tera-alkahest/alkahest-assets/releases/download/{region}/{File.Name}.zip")));

            using (var zip = ZipFile.OpenRead(zipName))
                zip.Entries.Single().ExtractToFile(File.FullName, true);

            System.IO.File.Delete(zipName);

            File.Refresh();
        }
    }
}
