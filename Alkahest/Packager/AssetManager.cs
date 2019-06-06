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

            _dc = new DataCenterAsset(assets, (JObject)JObject.Parse(GitHub.GetString(
                Configuration.AssetManifestUri))[Configuration.Region.ToString().ToLowerInvariant()]);
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
