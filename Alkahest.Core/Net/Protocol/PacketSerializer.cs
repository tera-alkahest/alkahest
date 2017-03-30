using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using Alkahest.Core.Data;
using Alkahest.Core.IO;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class PacketSerializer
    {
        const BindingFlags FieldFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.NonPublic |
            BindingFlags.Public;

        sealed class PacketFieldInfo
        {
            public PropertyInfo Property { get; }

            public PacketFieldAttribute Attribute { get; }

            public bool IsPrimitive { get; }

            public bool IsArray { get; }

            public bool IsByteArray { get; }

            public bool IsString { get; }

            public Action<TeraBinaryWriter, object> PrimitiveSerializer { get; }

            public Func<TeraBinaryReader, object> PrimitiveDeserializer { get; }

            public Func<object, object> ValueGetter { get; }

            public Action<object, object> ValueSetter { get; }

            public Action<object> EnumValidator { get; }

            public Func<IList> ArrayConstructor { get; }

            public Func<object> ElementConstructor { get; }

            public PacketFieldInfo(PropertyInfo property,
                PacketFieldAttribute attribute)
            {
                Property = property;
                Attribute = attribute;

                var type = property.PropertyType;

                IsPrimitive = IsPrimitiveType(type);

                var isArray = type.IsConstructedGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>);
                var elemType = isArray ? type.GetGenericArguments()[0] : null;
                var isByteArray = elemType == typeof(byte);

                IsArray = isArray && !isByteArray;
                IsByteArray = isArray && isByteArray;
                IsString = type == typeof(string);

                if (IsPrimitive)
                {
                    GetFunctions(attribute, type, out var s, out var d);

                    PrimitiveSerializer = s;
                    PrimitiveDeserializer = d;
                }

                ValueGetter = GetValueGetter(property);

                if (IsPrimitive || IsString)
                    ValueSetter = GetValueSetter(property);

                if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() == null)
                {
                    EnumValidator = val =>
                    {
                        if (!Enum.IsDefined(type, val))
                            throw new InvalidDataException(
                                $"Invalid enum value {val} encountered for {type}.");
                    };
                }

                if (IsArray)
                    ElementConstructor = GetDefaultConstructor(elemType);
            }

            static bool IsPrimitiveType(Type type)
            {
                if (type.IsEnum)
                    type = type.GetEnumUnderlyingType();

                return type.IsPrimitive ||
                    type == typeof(Vector3) ||
                    type == typeof(EntityId) ||
                    type == typeof(SkillId) ||
                    type == typeof(Angle);
            }

            static void GetFunctions(PacketFieldAttribute attribute, Type type,
                out Action<TeraBinaryWriter, object> serializer,
                out Func<TeraBinaryReader, object> deserializer)
            {
                if (type.IsEnum)
                    type = type.GetEnumUnderlyingType();

                if (type == typeof(bool))
                {
                    serializer = (w, v) => w.WriteBoolean((bool)v);
                    deserializer = r => r.ReadBoolean();
                }
                else if (type == typeof(byte))
                {
                    serializer = (w, v) => w.WriteByte((byte)v);
                    deserializer = r => r.ReadByte();
                }
                else if (type == typeof(sbyte))
                {
                    serializer = (w, v) => w.WriteSByte((sbyte)v);
                    deserializer = r => r.ReadSByte();
                }
                else if (type == typeof(ushort))
                {
                    if (attribute.IsUnknownArray)
                        serializer = (w, v) => w.WriteUInt16(0);
                    else
                        serializer = (w, v) => w.WriteUInt16((ushort)v);

                    deserializer = r => r.ReadUInt16();
                }
                else if (type == typeof(short))
                {
                    serializer = (w, v) => w.WriteInt16((short)v);
                    deserializer = r => r.ReadInt16();
                }
                else if (type == typeof(uint))
                {
                    serializer = (w, v) => w.WriteUInt32((uint)v);
                    deserializer = r => r.ReadUInt32();
                }
                else if (type == typeof(int))
                {
                    serializer = (w, v) => w.WriteInt32((int)v);
                    deserializer = r => r.ReadInt32();
                }
                else if (type == typeof(ulong))
                {
                    serializer = (w, v) => w.WriteUInt64((ulong)v);
                    deserializer = r => r.ReadUInt64();
                }
                else if (type == typeof(long))
                {
                    serializer = (w, v) => w.WriteInt64((long)v);
                    deserializer = r => r.ReadInt64();
                }
                else if (type == typeof(float))
                {
                    serializer = (w, v) => w.WriteSingle((float)v);
                    deserializer = r => r.ReadSingle();
                }
                else if (type == typeof(Vector3))
                {
                    serializer = (w, v) => w.WriteVector3((Vector3)v);
                    deserializer = r => r.ReadVector3();
                }
                else if (type == typeof(EntityId))
                {
                    serializer = (w, v) => w.WriteEntityId((EntityId)v);
                    deserializer = r => r.ReadEntityId();
                }
                else if (type == typeof(SkillId))
                {
                    serializer = (w, v) => w.WriteSkillId((SkillId)v);
                    deserializer = r => r.ReadSkillId();
                }
                else if (type == typeof(Angle))
                {
                    serializer = (w, v) => w.WriteAngle((Angle)v);
                    deserializer = r => r.ReadAngle();
                }
                else
                    throw Assert.Unreachable();
            }

            static Func<object, object> GetValueGetter(PropertyInfo property)
            {
                var thisParam = Expression.Parameter(typeof(object));

                return Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Property(
                            Expression.Convert(
                                thisParam, property.DeclaringType),
                            property),
                        typeof(object)),
                    thisParam).Compile();
            }

            static Action<object, object> GetValueSetter(PropertyInfo property)
            {
                var thisParam = Expression.Parameter(typeof(object));
                var valueParam = Expression.Parameter(typeof(object));

                return Expression.Lambda<Action<object, object>>(
                    Expression.Assign(
                        Expression.Property(
                            Expression.Convert(
                                thisParam, property.DeclaringType),
                            property),
                        Expression.Convert(
                            valueParam, property.PropertyType)),
                    thisParam, valueParam).Compile();
            }

            static Func<object> GetDefaultConstructor(Type type)
            {
                return Expression.Lambda<Func<object>>(
                    Expression.New(type)
                    ).Compile();
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

        PacketFieldInfo[] GetPacketFields(Type type)
        {
            return _info.GetOrAdd(type, t =>
            {
                return (from prop in t.GetProperties(FieldFlags)
                        let attr = prop.GetCustomAttribute<PacketFieldAttribute>()
                        where attr != null
                        orderby prop.MetadataToken
                        select new PacketFieldInfo(prop, attr)).ToArray();
            });
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

            foreach (var info in fields)
            {
                if (info.IsByteArray)
                {
                    var offset = reader.ReadOffset();
                    var count = reader.ReadUInt16();

                    var list = (List<byte>)info.ValueGetter(target);

                    list.Clear();

                    if (count == 0)
                        continue;

                    reader.Seek(offset, (r, op) =>
                        list.AddRange(r.ReadBytes(count)));
                }
                else if (info.IsArray)
                {
                    var count = reader.ReadUInt16();
                    var offset = reader.ReadOffset();
                    var list = (IList)info.ValueGetter(target);

                    list.Clear();

                    if (count == 0)
                        continue;

                    var next = offset;

                    for (var i = 0; i < count; i++)
                    {
                        reader.Seek(next, (r, op) =>
                        {
                            r.ReadOffset();
                            next = r.ReadOffset();

                            var elem = info.ElementConstructor();

                            DeserializeObject(r, elem);
                            list.Add(elem);
                        });
                    }
                }
                else if (info.IsString)
                {
                    var offset = reader.ReadOffset();

                    info.ValueSetter(target, reader.Seek(offset,
                        (r, op) => r.ReadString()));
                }
                else
                {
                    var val = info.PrimitiveDeserializer(reader);

                    info.EnumValidator?.Invoke(val);
                    info.ValueSetter(target, val);
                }
            }
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
            var offsets = new Dictionary<PacketFieldInfo, int>();

            foreach (var info in fields)
            {
                if (info.IsByteArray)
                {
                    offsets.Add(info, writer.Position);
                    writer.WriteUInt16(0);
                    writer.WriteUInt16(
                        (ushort)((List<byte>)info.ValueGetter(source)).Count);
                }
                else if (info.IsArray)
                {
                    writer.WriteUInt16(
                        (ushort)((IList)info.ValueGetter(source)).Count);
                    offsets.Add(info, writer.Position);
                    writer.WriteUInt16(0);
                }
                else if (info.IsString)
                {
                    offsets.Add(info, writer.Position);
                    writer.WriteUInt16(0);
                }
                else
                    info.PrimitiveSerializer(writer, info.ValueGetter(source));
            }

            foreach (var info in fields.Where(x => x.IsString))
            {
                writer.Seek(offsets[info], (w, op) => w.WriteOffset(op));
                writer.WriteString((string)info.ValueGetter(source) ?? string.Empty);
            }

            foreach (var info in fields.Where(x => x.IsByteArray))
            {
                var list = (List<byte>)info.ValueGetter(source);

                if (list.Count == 0)
                    continue;

                writer.Seek(offsets[info], (w, op) => w.WriteOffset(op));
                writer.WriteBytes(list.ToArray());
            }

            foreach (var info in fields.Where(x => x.IsArray))
            {
                var list = (IList)info.ValueGetter(source);

                if (list.Count == 0)
                    continue;

                writer.Seek(offsets[info], (w, op) => w.WriteOffset(op));

                var markers = new Stack<int>();

                for (var i = 0; i < list.Count; i++)
                {
                    var isLast = i == list.Count - 1;

                    writer.WriteOffset(writer.Position);

                    if (!isLast)
                        markers.Push(writer.Position);

                    writer.WriteUInt16(0);

                    SerializeObject(writer, list[i]);

                    if (!isLast)
                        markers.Push(writer.Position);
                }

                for (var i = 0; i < markers.Count / 2; i++)
                {
                    var afterElemPos = markers.Pop();
                    var nextPos = markers.Pop();

                    writer.Seek(nextPos, (w, op) => w.WriteOffset(afterElemPos));
                }
            }
        }
    }
}
