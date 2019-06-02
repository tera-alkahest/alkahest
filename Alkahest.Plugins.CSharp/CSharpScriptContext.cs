using Alkahest.Core.Data;
using Alkahest.Core.Logging;

namespace Alkahest.Plugins.CSharp
{
    public sealed class CSharpScriptContext
    {
        public DataCenter Data { get; }

        public Log Log { get; }

        internal CSharpScriptContext(DataCenter data, Log log)
        {
            Data = data;
            Log = log;
        }
    }
}
