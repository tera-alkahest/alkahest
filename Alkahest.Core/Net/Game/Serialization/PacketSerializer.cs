using Alkahest.Core.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Net.Game.Serialization
{
    public abstract class PacketSerializer
    {
        internal const BindingFlags FieldFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Public;

        public Region Region { get; }

        public GameMessageTable GameMessages { get; }

        public SystemMessageTable SystemMessages { get; }

        readonly ConcurrentDictionary<Type, IReadOnlyList<PacketFieldInfo>> _info =
            new ConcurrentDictionary<Type, IReadOnlyList<PacketFieldInfo>>();

        readonly IReadOnlyDictionary<ushort, Func<Packet>> _packetCreators;

        protected PacketSerializer(Region region, GameMessageTable gameMessages,
            SystemMessageTable systemMessages)
        {
            Region = region.CheckValidity(nameof(region));
            GameMessages = gameMessages ?? throw new ArgumentNullException(nameof(gameMessages));
            SystemMessages = systemMessages ?? throw new ArgumentNullException(nameof(systemMessages));

            var creators = new Dictionary<ushort, Func<Packet>>();
            using var container = new CompositionContainer(new AssemblyCatalog(
                Assembly.GetExecutingAssembly()), true);
            var exports = container.GetExports<Func<Packet>, IPacketMetadata>(
                PacketAttribute.ThisContractName);

            foreach (var lazy in exports.Where(x => gameMessages.NameToCode.ContainsKey(x.Metadata.OpCode)))
                creators.Add(gameMessages.NameToCode[lazy.Metadata.OpCode], lazy.Value);

            _packetCreators = creators;
        }

        public bool IsKnown(ushort opCode)
        {
            return _packetCreators.ContainsKey(opCode);
        }

        public Type GetType(ushort opCode)
        {
            _packetCreators.TryGetValue(opCode, out var creator);

            return creator?.Method.DeclaringType;
        }

        public Packet Create(ushort opCode)
        {
            _packetCreators.TryGetValue(opCode, out var creator);

            return creator?.Invoke();
        }

        protected abstract PacketFieldInfo CreateFieldInfo(PropertyInfo property,
            PacketFieldAttribute attribute);

        protected IReadOnlyList<T> GetPacketFields<T>(Type type)
            where T : PacketFieldInfo
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return _info.GetOrAdd(type, t =>
            {
                return (from prop in t.GetProperties(FieldFlags)
                        let attr = prop.GetCustomAttribute<PacketFieldAttribute>()
                        where attr != null
                        where attr.Regions.Length == 0 ||
                            attr.Regions.Contains(Region)
                        orderby prop.MetadataToken
                        select CreateFieldInfo(prop, attr)).ToArray();
            }).Cast<T>().ToArray();
        }

        protected abstract void OnSerialize(GameBinaryWriter writer, Packet packet);

        public byte[] Serialize(Packet packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            packet.OnSerialize(this);

            using var writer = new GameBinaryWriter();

            OnSerialize(writer, packet);

            return writer.ToArray();
        }

        protected abstract void OnDeserialize(GameBinaryReader reader, Packet packet);

        public void Deserialize(byte[] payload, Packet packet)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            using var reader = new GameBinaryReader(payload);

            OnDeserialize(reader, packet);

            packet.OnDeserialize(this);
        }
    }
}
