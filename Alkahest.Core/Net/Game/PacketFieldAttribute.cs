using System;

namespace Alkahest.Core.Net.Game
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PacketFieldAttribute : Attribute
    {
        public Region[] Regions { get; }

        public bool IsUnknownArray { get; set; }

        internal PacketFieldAttribute(params Region[] regions)
        {
            Regions = regions;
        }
    }
}
