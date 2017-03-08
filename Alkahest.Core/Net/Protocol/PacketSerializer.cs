using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
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

            public bool IsPrimitive => !Property.PropertyType.IsArray &&
                Property.PropertyType != typeof(string);

            public bool IsCount => Property.Name.EndsWith(CountNameSuffix);

            public bool IsOffset => Property.Name.EndsWith(OffsetNameSuffix);

            public PacketFieldInfo(PropertyInfo property,
                PropertyInfo countField, PropertyInfo offsetField)
            {
                Property = property;
                CountField = countField;
                OffsetField = offsetField;
            }
        }

        public OpCodeTable GameMessages { get; }

        public OpCodeTable SystemMessages { get; }

        readonly ConcurrentDictionary<Type, PacketFieldInfo[]> _info =
            new ConcurrentDictionary<Type, PacketFieldInfo[]>();

        readonly Dictionary<ushort, Func<Packet>> _packetCreators =
            new Dictionary<ushort, Func<Packet>>();

        public PacketSerializer(OpCodeTable gameMessages,
            OpCodeTable systemMessages)
        {
            GameMessages = gameMessages;
            SystemMessages = systemMessages;

            using (var container = new CompositionContainer(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()), true))
                foreach (var lazy in container.GetExports<Func<Packet>,
                    IPacketMetadata>(PacketAttribute.ThisContractName))
                    _packetCreators.Add(gameMessages.NameToOpCode[lazy.Metadata.OpCode],
                        lazy.Value);
        }

        public Packet Create(ushort opCode)
        {
            Func<Packet> creator;

            _packetCreators.TryGetValue(opCode, out creator);

            return creator?.Invoke();
        }

        public void Deserialize(byte[] payload, Packet packet)
        {
            using (var reader = new TeraBinaryReader(payload))
                DeserializeObject(reader, packet);

            packet.OnDeserialize();
        }

        void DeserializeObject(TeraBinaryReader reader, object target)
        {
            var fields = GetPacketFields(target.GetType());

            foreach (var info in fields.Where(x => x.IsPrimitive))
                info.Property.SetValue(target,
                    DeserializePrimitive(reader, info.Property.PropertyType));

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
                                reader.Position = next - PacketHeader.HeaderSize;
                        }
                    });

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
            else
                throw new Exception();

            return value;
        }

        public byte[] Serialize(Packet packet)
        {
            packet.OnSerialize();

            using (var writer = new TeraBinaryWriter())
            {
                SerializeObject(writer, packet);

                return writer.BaseStream.ToArray();
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

                SerializePrimitive(writer, info.Property.GetValue(source));
            }

            foreach (var info in fields.Where(x => !x.IsPrimitive))
            {
                var type = info.Property.PropertyType;

                writer.Seek(offsets[info.Property.Name + OffsetNameSuffix],
                    (w, op) => w.Write((ushort)(op + PacketHeader.HeaderSize)));

                if (type.IsArray)
                {
                    var array = (Array)info.Property.GetValue(source);

                    writer.Seek(counts[info.Property.Name + CountNameSuffix],
                        (w, op) => w.Write((ushort)array.Length));

                    var elemType = type.GetElementType();
                    var markers = new Stack<int>();

                    for (var i = 0; i < array.Length; i++)
                    {
                        var isLast = i == array.Length - 1;

                        if (!IsByte(elemType))
                        {
                            writer.Write((ushort)(writer.Position +
                                PacketHeader.HeaderSize));

                            if (!isLast)
                                markers.Push(writer.Position);

                            writer.Write((ushort)0);
                        }

                        var value = array.GetValue(i);

                        if (elemType.IsPrimitive)
                            SerializePrimitive(writer, value);
                        else
                            SerializeObject(writer, value);

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
                                w.Write((ushort)(afterElemPos + PacketHeader.HeaderSize)));
                        }
                    }
                }
                else
                    writer.Write((string)info.Property.GetValue(source));
            }
        }

        static void SerializePrimitive(TeraBinaryWriter writer, object value)
        {
            var type = value.GetType();

            if (type == typeof(bool))
                writer.Write((bool)value);
            else if (type == typeof(byte))
                writer.Write((byte)value);
            else if (type == typeof(sbyte))
                writer.Write((sbyte)value);
            else if (type == typeof(ushort))
                writer.Write((ushort)value);
            else if (type == typeof(short))
                writer.Write((short)value);
            else if (type == typeof(uint))
                writer.Write((uint)value);
            else if (type == typeof(int))
                writer.Write((int)value);
            else if (type == typeof(ulong))
                writer.Write((ulong)value);
            else if (type == typeof(long))
                writer.Write((long)value);
            else if (type == typeof(float))
                writer.Write((float)value);
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
