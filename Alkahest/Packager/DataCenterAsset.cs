using Newtonsoft.Json.Linq;
using System;
using System.IO;
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

            System.IO.File.WriteAllBytes(File.FullName, GitHub.GetBytes(
                new Uri($"https://github.com/tera-alkahest/alkahest-assets/releases/download/{region}/{File.Name}")));
            File.Refresh();
        }
    }
}
