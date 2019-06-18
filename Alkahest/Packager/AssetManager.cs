using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Packager
{
    sealed class AssetManager
    {
        static readonly Log _log = new Log(typeof(AssetManager));

        readonly IReadOnlyList<IAsset> _dataCenters;

        public AssetManager()
        {
            var assets = Configuration.AssetDirectory;

            Directory.CreateDirectory(assets);

            _log.Info("Fetching asset manifest...");

            var json = JObject.Parse(GitHub.GetString(Configuration.AssetManifestUri));

            _log.Info("Loading local assets...");

            var dcs = new List<IAsset>();

            foreach (var region in Configuration.Regions)
                dcs.Add(new DataCenterAsset(assets, region,
                    (JObject)json[region.ToString().ToLowerInvariant()]));

            _dataCenters = dcs;
        }

        static void Update(IAsset asset)
        {
            if (!asset.CheckIfLatest())
            {
                _log.Basic("Asset {0} is out of date; updating...", asset.File);

                asset.Update();

                _log.Info("Asset {0} updated", asset.File);
            }
            else
                _log.Info("Asset {0} is up to date", asset.File);
        }

        void UpdateDataCenters()
        {
            foreach (var dc in _dataCenters.OfType<DataCenterAsset>())
                Update(dc);
        }

        public void Update(AssetKind kind)
        {
            switch (kind)
            {
                case AssetKind.DataCenter:
                    UpdateDataCenters();
                    break;
            }
        }

        public void UpdateAll()
        {
            foreach (var kind in (AssetKind[])Enum.GetValues(typeof(AssetKind)))
                Update(kind);
        }
    }
}
