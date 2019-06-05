using System;

namespace Alkahest.Core.Net.Game.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PacketFieldAttribute : Attribute
    {
        public Region[] Regions { get; }

        public bool IsUnknownArray { get; set; }

        public bool IsSimpleSkill { get; set; }

        internal PacketFieldAttribute(params Region[] regions)
        {
            Regions = regions;
        }
    }
}
