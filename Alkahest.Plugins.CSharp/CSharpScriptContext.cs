using Alkahest.Core.Data;
using Alkahest.Core.Logging;

namespace Alkahest.Plugins.CSharp
{
    public sealed class CSharpScriptContext
    {
        public string Name { get; }

        public string Path { get; }

        public Log Log { get; }

        public DataCenter Data { get; }

        internal CSharpScriptContext(string name, string path, Log log, DataCenter data)
        {
            Name = name;
            Path = path;
            Data = data;
            Log = log;
        }
    }
}
