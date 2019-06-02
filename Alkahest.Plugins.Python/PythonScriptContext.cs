using Alkahest.Core.Data;
using Alkahest.Core.Logging;

namespace Alkahest.Plugins.Python
{
    public sealed class PythonScriptContext
    {
        public DataCenter Data { get; }

        public Log Log { get; }

        internal PythonScriptContext(DataCenter data, Log log)
        {
            Data = data;
            Log = log;
        }
    }
}
