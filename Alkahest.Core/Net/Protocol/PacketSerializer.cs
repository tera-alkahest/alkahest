using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Alkahest.Core.Data;
using Alkahest.Core.IO;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class PacketSerializer
    {
        const string CountNameSuffix = "Count";

        const string OffsetNameSuffix = "Offset";

        const BindingFlags FieldFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public;

        sealed class PacketFieldInfo
        {
            public PropertyInfo Property { get; }

            public PropertyInfo CountField { get; }

            public PropertyInfo OffsetField { get; }

            public bool IsPrimitive { get; }

            public bool IsCount { get; }

            public bool IsOffset { get; }

            public PacketFieldInfo(PropertyInfo property,
                PropertyInfo countField, PropertyInfo offsetField)
            {
                Property = property;
                CountField = countField;
                OffsetField = offsetField;
                IsPrimitive = !property.PropertyType.IsArray &&
                    property.PropertyType != typeof(string);
                IsCount = property.Name.EndsWith(CountNameSuffix);
                IsOffset = property.Name.EndsWith(OffsetNameSuffix);
            }
        }

        public OpCodeTable GameMessages { get; }

        public OpCodeTable SystemMessages { get; }

        readonly ConcurrentDictionary<Type, PacketFieldInfo[]> _info =
            new ConcurrentDictionary<Type, PacketFieldInfo[]>();

        readonly IReadOnlyDictionary<ushort, Func<Packet>> _packetCreators;

        public PacketSerializer(OpCodeTable gameMessages,
            OpCodeTable systemMessages)
        {
            GameMessages = gameMessages;
            SystemMessages = systemMessages;

            var creators = new Dictionary<ushort, Func<Packet>>();

            using (var container = new CompositionContainer(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()), true))
                foreach (var lazy in container.GetExports<Func<Packet>,
                    IPacketMetadata>(PacketAttribute.ThisContractName))
                    creators.Add(gameMessages.NameToOpCode[lazy.Metadata.OpCode],
                        lazy.Value);

            _packetCreators = creators;
        }

        public Packet Create(ushort opCode)
        {
            _packetCreators.TryGetValue(opCode, out var creator);

            return creator?.Invoke();
        }

        public void Deserialize(byte[] payload, Packet packet)
        {
            using (var reader = new TeraBinaryReader(payload))
                DeserializeObject(reader, packet);

            packet.OnDeserialize(this);
        }

        void DeserializeObject(TeraBinaryReader reader, object target)
        {
            var fields = GetPacketFields(target.GetType());

            foreach (var info in fields.Where(x => x.IsPrimitive))
            {
                var type = info.Property.PropertyType;

                if (type.IsEnum)
                    type = type.GetEnumUnderlyingType();

                info.Property.SetValue(target, DeserializePrimitive(reader, type));
            }

            foreach (var info in fields.Where(x => !x.IsPrimitive))
            {
                var type = info.Property.PropertyType;
                var offset = (ushort)info.OffsetField.GetValue(target);

                object value;

                if (type.IsArray)
                {
                    var count = (ushort)info.CountField.GetValue(target);
                    var array = (Array)Activator.CreateInstance(type, (int)count);
                    var elemType = type.GetElementType();

                    if (elemType.IsEnum)
                        elemType = elemType.GetEnumUnderlyingType();

                    if (offset != 0)
                    {
                        reader.Seek(offset - PacketHeader.HeaderSize, (r, op) =>
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var next = 0;

                                if (!IsByte(elemType))
                                {
                                    reader.ReadUInt16();
                                    next = reader.ReadUInt16();
                                }

                                object elem;

                                if (!elemType.IsPrimitive)
                                {
                                    elem = Activator.CreateInstance(elemType);
                                    DeserializeObject(reader, elem);
                                }
                                else
                                    elem = DeserializePrimitive(reader, elemType);

                                array.SetValue(elem, i);

                                if (!IsByte(elemType) && i != count - 1)
                                    reader.Position = next -
                                        PacketHeader.HeaderSize;
                            }
                        });
                    }

                    value = array;
                }
                else
                    value = reader.Seek(offset - PacketHeader.HeaderSize,
                        (r, op) => r.ReadString());

                info.Property.SetValue(target, value);
            }
        }

        static object DeserializePrimitive(TeraBinaryReader reader, Type type)
        {
            object value;

            if (type == typeof(bool))
                value = reader.ReadBoolean();
            else if (type == typeof(byte))
                value = reader.ReadByte();
            else if (type == typeof(sbyte))
                value = reader.ReadSByte();
            else if (type == typeof(ushort))
                value = reader.ReadUInt16();
            else if (type == typeof(short))
                value = reader.ReadInt16();
            else if (type == typeof(uint))
                value = reader.ReadUInt32();
            else if (type == typeof(int))
                value = reader.ReadInt32();
            else if (type == typeof(ulong))
                value = reader.ReadUInt64();
            else if (type == typeof(long))
                value = reader.ReadInt64();
            else if (type == typeof(float))
                value = reader.ReadSingle();
            else if (type == typeof(Vector3))
                value = reader.ReadVector3();
            else if (type == typeof(EntityId))
                value = reader.ReadEntityId();
            else if (type == typeof(SkillId))
                value = reader.ReadSkillId();
            else if (type == typeof(Angle))
                value = reader.ReadAngle();
            else
                throw Assert.Unreachable();

            return value;
        }

        public byte[] Serialize(Packet packet)
        {
            packet.OnSerialize(this);

            using (var writer = new TeraBinaryWriter())
            {
                SerializeObject(writer, packet);

                return writer.Stream.ToArray();
            }
        }

        void SerializeObject(TeraBinaryWriter writer, object source)
        {
            var fields = GetPacketFields(source.GetType());
            var counts = new Dictionary<string, int>();
            var offsets = new Dictionary<string, int>();

            foreach (var info in fields.Where(x => x.IsPrimitive))
            {
                if (info.IsCount)
                    counts.Add(info.Property.Name, writer.Position);
                else if (info.IsOffset)
                    offsets.Add(info.Property.Name, writer.Position);

                if (info.IsCount || info.IsOffset)
                    writer.WriteUInt16(0);
                else
                    SerializePrimitive(writer, info.Property.GetValue(source));
            }

            foreach (var info in fields.Where(x => !x.IsPrimitive))
            {
                var type = info.Property.PropertyType;
                var value = info.Property.GetValue(source);
                var array = value as Array;
                var noOffset = array != null && array.Length == 0;

                writer.Seek(offsets[info.Property.Name + OffsetNameSuffix],
                    (w, op) => w.WriteUInt16((ushort)(noOffset ?
                        0 : op + PacketHeader.HeaderSize)));

                if (type.IsArray)
                {
                    writer.Seek(counts[info.Property.Name + CountNameSuffix],
                        (w, op) => w.WriteUInt16((ushort)array.Length));

                    var elemType = type.GetElementType();

                    if (elemType.IsEnum)
                        elemType = elemType.GetEnumUnderlyingType();

                    var markers = new Stack<int>();

                    for (var i = 0; i < array.Length; i++)
                    {
                        var isLast = i == array.Length - 1;

                        if (!IsByte(elemType))
                        {
                            writer.WriteUInt16((ushort)(writer.Position +
                                PacketHeader.HeaderSize));

                            if (!isLast)
                                markers.Push(writer.Position);

                            writer.WriteUInt16(0);
                        }

                        var elem = array.GetValue(i);

                        if (elemType.IsPrimitive)
                            SerializePrimitive(writer, elem);
                        else
                            SerializeObject(writer, elem);

                        if (!IsByte(elemType) && !isLast)
                            markers.Push(writer.Position);
                    }

                    if (!IsByte(elemType))
                    {
                        for (var i = 0; i < markers.Count / 2; i++)
                        {
                            var afterElemPos = markers.Pop();
                            var nextPos = markers.Pop();

                            writer.Seek(nextPos, (w, op) =>
                                w.WriteUInt16((ushort)(afterElemPos +
                                    PacketHeader.HeaderSize)));
                        }
                    }
                }
                else
                    writer.WriteString((string)value);
            }
        }

        static void SerializePrimitive(TeraBinaryWriter writer, object value)
        {
            var type = value.GetType();

            if (type.IsEnum)
                type = type.GetEnumUnderlyingType();

            if (type == typeof(bool))
                writer.WriteBoolean((bool)value);
            else if (type == typeof(byte))
                writer.WriteByte((byte)value);
            else if (type == typeof(sbyte))
                writer.WriteSByte((sbyte)value);
            else if (type == typeof(ushort))
                writer.WriteUInt16((ushort)value);
            else if (type == typeof(short))
                writer.WriteInt16((short)value);
            else if (type == typeof(uint))
                writer.WriteUInt32((uint)value);
            else if (type == typeof(int))
                writer.WriteInt32((int)value);
            else if (type == typeof(ulong))
                writer.WriteUInt64((ulong)value);
            else if (type == typeof(long))
                writer.WriteInt64((long)value);
            else if (type == typeof(float))
                writer.WriteSingle((float)value);
            else if (type == typeof(Vector3))
                writer.WriteVector3((Vector3)value);
            else if (type == typeof(EntityId))
                writer.WriteEntityId((EntityId)value);
            else if (type == typeof(SkillId))
                writer.WriteSkillId((SkillId)value);
            else if (type == typeof(Angle))
                writer.WriteAngle((Angle)value);
            else
                throw Assert.Unreachable();
        }

        static bool IsByte(Type type)
        {
            return type == typeof(bool) ||
                type == typeof(byte) ||
                type == typeof(sbyte);
        }

        PacketFieldInfo[] GetPacketFields(Type type)
        {
            return _info.GetOrAdd(type, t =>
            {
                return t.GetProperties(FieldFlags)
                    .Where(p => p.GetCustomAttribute<PacketFieldAttribute>() != null)
                    .OrderBy(p => p.MetadataToken)
                    .Select(p => new PacketFieldInfo(p,
                        t.GetProperty(p.Name + CountNameSuffix, FieldFlags),
                        t.GetProperty(p.Name + OffsetNameSuffix, FieldFlags)))
                    .ToArray();
            });
        }
    }
}
