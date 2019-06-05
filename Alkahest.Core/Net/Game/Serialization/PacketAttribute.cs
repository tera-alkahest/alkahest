using System;

namespace Alkahest.Core.Net.Game.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class PacketAttribute : Attribute
    {
        public string Name { get; }

        internal PacketAttribute(string name)
        {
            Name = name;
        }
    }
}
