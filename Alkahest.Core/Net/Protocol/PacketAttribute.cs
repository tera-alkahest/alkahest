using System;
using System.ComponentModel.Composition;

namespace Alkahest.Core.Net.Protocol
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PacketAttribute : ExportAttribute
    {
        internal const string ThisContractName =
            "Alkahest.Core.Net.Protocol.Packet";

        public string OpCode { get; }

        internal PacketAttribute(string opCode)
            : base(ThisContractName)
        {
            OpCode = opCode ?? throw new ArgumentNullException(nameof(opCode));
        }
    }
}
