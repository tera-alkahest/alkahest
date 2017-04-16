using System;

namespace Alkahest.Core.Plugins
{
    public class PluginException : Exception
    {
        public PluginException(string message)
            : base(message)
        {
        }
    }
}
