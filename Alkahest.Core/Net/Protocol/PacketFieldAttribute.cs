using System;

namespace Alkahest.Core.Net.Protocol
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    sealed class PacketFieldAttribute : Attribute
    {
        public bool IsUnknownArray { get; set; }
    }
}
