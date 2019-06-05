using System;

namespace Alkahest.Core.Net.Game.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class PacketFieldOptionsAttribute : Attribute
    {
        internal Region[] Regions { get; }

        public bool Skip { get; set; }

        public bool IsUnknownArray { get; set; }

        public bool IsSimpleSkill { get; set; }

        internal PacketFieldOptionsAttribute(params Region[] regions)
        {
            Regions = regions;
        }
    }
}
