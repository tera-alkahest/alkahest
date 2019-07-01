using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Net.Game.Serialization
{
    public abstract class PacketSerializer
    {
        static readonly Log _log = new Log(typeof(PacketSerializer));

        public Region Region { get; }

        public GameMessageTable GameMessages { get; }

        public SystemMessageTable SystemMessages { get; }

        readonly IReadOnlyDictionary<Type, PacketInfo> _byType;

        readonly IReadOnlyDictionary<string, PacketInfo> _byName;

        readonly IReadOnlyDictionary<ushort, PacketInfo> _byCode;

        protected PacketSerializer(Region region, GameMessageTable gameMessages,
            SystemMessageTable systemMessages)
        {
            Region = region.CheckValidity(nameof(region));
            GameMessages = gameMessages ?? throw new ArgumentNullException(nameof(gameMessages));
            SystemMessages = systemMessages ?? throw new ArgumentNullException(nameof(systemMessages));

            var byType = new Dictionary<Type, PacketInfo>();
            var byName = new Dictionary<string, PacketInfo>();
            var byCode = new Dictionary<ushort, PacketInfo>();

            void RegisterType(Type type, PacketAttribute attribute, ushort? code)
            {
                var info = new PacketInfo(type, attribute,
                    (from prop in type.GetProperties()
                     let opts = prop.GetCustomAttribute<PacketFieldOptionsAttribute>()
                     where opts == null || (!opts.Skip && (opts.Regions.Length == 0 || opts.Regions.Contains(region)))
                     orderby prop.MetadataToken
                     select CreateFieldInfo(prop, opts)).ToArray());

                byType.Add(type, info);

                if (code is ushort c)
                {
                    byName.Add(attribute.Name, info);
                    byCode.Add(c, info);
                }

                foreach (var field in info.Fields.Where(x => x.IsArray))
                    RegisterType(field.Property.PropertyType.GetGenericArguments()[0], null, null);
            }

            foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes)
            {
                var attr = type.GetCustomAttribute<PacketAttribute>();

                if (attr == null)
                    continue;

                if (!gameMessages.NameToCode.TryGetValue(attr.Name, out var code))
                {
                    _log.Warning("Game message {0} not mapped to a code; ignoring definition", attr.Name);
                    continue;
                }

                RegisterType(type, attr, code);
            }

            _byType = byType;
            _byName = byName;
            _byCode = byCode;
        }

        public PacketInfo GetPacketInfo(Type type)
        {
            return _byType.GetValueOrDefault(type);
        }

        public PacketInfo GetPacketInfo(string name)
        {
            return _byName.GetValueOrDefault(name);
        }

        public PacketInfo GetPacketInfo(ushort code)
        {
            return _byCode.GetValueOrDefault(code);
        }

        public SerializablePacket Create(Type type)
        {
            var info = _byType.GetValueOrDefault(type);

            return info != null ? OnCreate(info) : null;
        }

        public SerializablePacket Create(string name)
        {
            var info = _byName.GetValueOrDefault(name);

            return info != null ? OnCreate(info) : null;
        }

        public SerializablePacket Create(ushort code)
        {
            var info = _byCode.GetValueOrDefault(code);

            return info != null ? OnCreate(info) : null;
        }

        protected abstract SerializablePacket OnCreate(PacketInfo info);

        protected abstract PacketFieldInfo CreateFieldInfo(PropertyInfo property,
            PacketFieldOptionsAttribute attribute);

        protected abstract void OnSerialize(GameBinaryWriter writer, PacketInfo info,
            SerializablePacket packet);

        public byte[] Serialize(SerializablePacket packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            packet.OnSerialize(this);

            using var writer = new GameBinaryWriter();

            OnSerialize(writer, _byType[packet.GetType()], packet);

            return writer.ToArray();
        }

        protected abstract void OnDeserialize(GameBinaryReader reader, PacketInfo info,
            SerializablePacket packet);

        public void Deserialize(byte[] buffer, int index, int count, SerializablePacket packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            using var reader = new GameBinaryReader(buffer, index, count);

            OnDeserialize(reader, _byType[packet.GetType()], packet);

            packet.OnDeserialize(this);
        }
    }
}
