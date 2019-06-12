using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace Alkahest.Packager
{
    sealed class DataCenterAsset
    {
        public FileInfo File { get; }

        public string Hash { get; }

        public DataCenterAsset(string directory, JObject obj)
        {
            File = new FileInfo(Path.Combine(directory, (string)obj["name"]));
            Hash = (string)obj["sha1"];
        }

        public bool CheckIfLatest()
        {
            if (!File.Exists)
                return false;

            using var sha = new SHA1Managed();

            return Hash == BitConverter.ToString(sha.ComputeHash(System.IO.File.ReadAllBytes(File.FullName)))
                .Replace("-", string.Empty).ToLowerInvariant();
        }

        public void Update()
        {
            var region = Configuration.Region.ToString().ToLowerInvariant();
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
