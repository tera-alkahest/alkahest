using Alkahest.Core.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Alkahest.Packager
{
    sealed class AssetManager
    {
        static readonly Log _log = new Log(typeof(AssetManager));

        readonly IReadOnlyList<DataCenterAsset> _dataCenters;

        public AssetManager()
        {
            var assets = Configuration.AssetDirectory;

            Directory.CreateDirectory(assets);

            _log.Info("Fetching asset manifest...");

            var dcs = new List<DataCenterAsset>();

            foreach (var region in Configuration.Regions)
                dcs.Add(new DataCenterAsset(assets, region, (JObject)JObject.Parse(GitHub.GetString(
                    Configuration.AssetManifestUri))[region.ToString().ToLowerInvariant()]));

            _dataCenters = dcs;
        }

        void UpdateDataCenters()
        {
            foreach (var dc in _dataCenters)
            {
                if (!dc.CheckIfLatest())
                {
                    _log.Basic("Asset {0} is out of date; updating...", dc.File);

                    dc.Update();

                    _log.Info("Asset {0} updated", dc.File);
                }
                else
                    _log.Info("Asset {0} is up to date", dc.File);
            }
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
