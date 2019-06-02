using Alkahest.Core.Data;
using System;

namespace Alkahest.Core.Plugins
{
    public sealed class PluginContext
    {
        public DataCenter Data { get; }

        public PluginContext(DataCenter data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
