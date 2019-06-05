using System;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Serialization
{
    public sealed class PacketInfo
    {
        public Type Type { get; }

        public PacketAttribute Attribute { get; }

        public IReadOnlyList<PacketFieldInfo> Fields { get; }

        internal PacketInfo(Type type, PacketAttribute attribute, IReadOnlyList<PacketFieldInfo> fields)
        {
            Type = type;
            Attribute = attribute;
            Fields = fields;
        }
    }
}
