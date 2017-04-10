using System;

namespace Alkahest.Core.Net.Protocol
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PacketFieldAttribute : Attribute
    {
        public bool IsUnknownArray { get; set; }

        public int MinVersion { get; set; }

        public int MaxVersion { get; set; }
    }
}
