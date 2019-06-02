using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Alkahest.Packager
{
    sealed class AssetManager
    {
        static readonly Log _log = new Log(typeof(AssetManager));

        readonly DataCenterAsset _dc;

        public AssetManager()
        {
            var assets = Configuration.AssetDirectory;

            Directory.CreateDirectory(assets);

            _log.Info("Fetching asset manifest...");

            var json = GitHub.GetString(Configuration.AssetManifestUri);
            var manifest = JObject.Parse(json);
            var region = Configuration.Region.ToString().ToLowerInvariant();
            var obj = manifest[region];

            if (obj == null)
                _log.Warning("{0} data center file not available", region);
            else
                _dc = new DataCenterAsset(assets, (JObject)obj);
        }

        public void UpdateDataCenter()
        {
            if (_dc == null)
                return;

            if (!_dc.CheckIfLatest())
            {
                _log.Basic("Asset {0} is out of date; updating...", _dc.File);

                _dc.Update();

                _log.Info("Asset {0} updated", _dc.File);
            }
            else
                _log.Info("Asset {0} is up to date", _dc.File);
        }

        public void UpdateAll()
        {
            UpdateDataCenter();
        }
    }
}
