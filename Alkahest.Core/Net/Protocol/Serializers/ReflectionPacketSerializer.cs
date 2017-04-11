using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using Alkahest.Core.Data;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Protocol.OpCodes;

namespace Alkahest.Core.Net.Protocol.Serializers
{
    public sealed class ReflectionPacketSerializer : PacketSerializer
    {
        sealed class ReflectionPacketFieldInfo : PacketFieldInfo
        {
            static readonly Log _log = new Log(typeof(ReflectionPacketFieldInfo));

            public Action<TeraBinaryWriter, object> PrimitiveSerializer { get; }

            public Func<TeraBinaryReader, object> PrimitiveDeserializer { get; }

            public Action<object> EnumValidator { get; }

            public Func<IList> ArrayConstructor { get; }

            public Func<object> ElementConstructor { get; }

            public ReflectionPacketFieldInfo(
                PropertyInfo property, PacketFieldAttribute attribute)
                : base(property, attribute)
            {
                var type = property.PropertyType;

                if (IsPrimitive)
                {
                    GetFunctions(property, attribute, out var s, out var d);

                    PrimitiveSerializer = s;
                    PrimitiveDeserializer = d;
                }

                if (type.IsEnum && type.GetCustomAttribute<FlagsAttribute>() == null)
                {
                    EnumValidator = val =>
                    {
                        if (!Enum.IsDefined(type, val))
                            _log.Warning("Found unknown enum value {0} for field {1}.{2} ({3})",
                                val, property.DeclaringType.Name,
                                property.Name, type.Name);
                    };
                }

                if (IsArray)
                {
                    var ctor = type.GetGenericArguments()[0]
                        .GetConstructor(Type.EmptyTypes);
                    var empty = Array.Empty<object>();

                    ElementConstructor = () => ctor.Invoke(empty);
                }
            }

            static void GetFunctions(PropertyInfo property,
                PacketFieldAttribute attribute,
                out Action<TeraBinaryWriter, object> serializer,
                out Func<TeraBinaryReader, object> deserializer)
            {
                var dtype = property.DeclaringType;
                var type = property.PropertyType;

                if (type.IsEnum)
                    type = type.GetEnumUnderlyingType();

                if (type == typeof(bool))
                {
                    serializer = (w, v) => w.WriteBoolean((bool)v);
                    deserializer = r =>
                    {
                        var b = r.ReadByte();

                        if (b != 0 && b != 1)
                            _log.Warning("Found non-Boolean value {0} for field {1}.{2}",
                                b, dtype.Name, property.Name);

                        return b != 0;
                    };
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
                    {
                        serializer = (w, v) => w.WriteUInt16(0);
                        deserializer = r =>
                        {
                            var v = r.ReadUInt16();

                            if (v != 0)
                                _log.Warning("Found non-zero value {0} for unknown array field {1}.{2}",
                                    v, dtype.Name, property.Name);

                            return v;
                        };
                    }
                    else
                    {
                        serializer = (w, v) => w.WriteUInt16((ushort)v);
                        deserializer = r => r.ReadUInt16();
                    }
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
                    if (attribute.IsLocalSkill)
                    {
                        serializer = (w, v) => w.WriteLocalSkillId((SkillId)v);
                        deserializer = r => r.ReadLocalSkillId();
                    }
                    else
                    {
                        serializer = (w, v) => w.WriteSkillId((SkillId)v);
                        deserializer = r => r.ReadSkillId();
                    }
                }
                else if (type == typeof(Angle))
                {
                    serializer = (w, v) => w.WriteAngle((Angle)v);
                    deserializer = r => r.ReadAngle();
                }
                else if (type == typeof(TemplateId))
                {
                    serializer = (w, v) => w.WriteTemplateId((TemplateId)v);
                    deserializer = r => r.ReadTemplateId();
                }
                else
                    throw Assert.Unreachable();
            }
        }

        public ReflectionPacketSerializer(GameMessageTable gameMessages,
            SystemMessageTable systemMessages)
            : base(gameMessages, systemMessages)
        {
        }

        protected override PacketFieldInfo CreateFieldInfo(
            PropertyInfo property, PacketFieldAttribute attribute)
        {
            return new ReflectionPacketFieldInfo(property, attribute);
        }

        protected override void OnSerialize(TeraBinaryWriter writer,
            Packet packet)
        {
            SerializeObject(writer, packet);
        }

        void SerializeObject(TeraBinaryWriter writer, object source)
        {
            var fields = GetPacketFields<ReflectionPacketFieldInfo>(source.GetType());
            var offsets = new List<(ReflectionPacketFieldInfo, int)>();

            foreach (var info in fields)
            {
                if (info.IsByteArray)
                {
                    offsets.Add((info, writer.Position));
                    writer.WriteUInt16(0);
                    writer.WriteUInt16((ushort)((List<byte>)info.Property
                        .GetValue(source)).Count);
                }
                else if (info.IsArray)
                {
                    writer.WriteUInt16(
                        (ushort)((IList)info.Property.GetValue(source)).Count);
                    offsets.Add((info, writer.Position));
                    writer.WriteUInt16(0);
                }
                else if (info.IsString)
                {
                    offsets.Add((info, writer.Position));
                    writer.WriteUInt16(0);
                }
                else
                    info.PrimitiveSerializer(writer,
                        info.Property.GetValue(source));
            }

            foreach (var (info, offset) in offsets)
            {
                if (info.IsByteArray)
                {
                    var list = (List<byte>)info.Property.GetValue(source);

                    if (list.Count == 0)
                        continue;

                    writer.Seek(offset, (w, op) => w.WriteOffset(op));
                    writer.WriteBytes(list.ToArray());
                }
                else if (info.IsArray)
                {
                    var list = (IList)info.Property.GetValue(source);

                    if (list.Count == 0)
                        continue;

                    writer.Seek(offset, (w, op) => w.WriteOffset(op));

                    for (var i = 0; i < list.Count; i++)
                    {
                        var pos = writer.Position;

                        writer.WriteOffset(pos);
                        writer.WriteUInt16(0);

                        SerializeObject(writer, list[i]);

                        if (i != list.Count - 1)
                            writer.Seek(pos + sizeof(ushort),
                                (w, op) => w.WriteOffset(op));
                    }
                }
                else
                {
                    writer.Seek(offset, (w, op) => w.WriteOffset(op));
                    writer.WriteString((string)info.Property.GetValue(source) ??
                        string.Empty);
                }
            }
        }

        protected override void OnDeserialize(TeraBinaryReader reader,
            Packet packet)
        {
            DeserializeObject(reader, packet);
        }

        void DeserializeObject(TeraBinaryReader reader, object target)
        {
            foreach (var info in
                GetPacketFields<ReflectionPacketFieldInfo>(target.GetType()))
            {
                if (info.IsByteArray)
                {
                    var offset = reader.ReadOffset();
                    var count = reader.ReadUInt16();

                    var list = (List<byte>)info.Property.GetValue(target);

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
                    var list = (IList)info.Property.GetValue(target);

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

                    info.Property.SetValue(target, reader.Seek(offset,
                        (r, op) => r.ReadString()));
                }
                else
                {
                    var val = info.PrimitiveDeserializer(reader);

                    info.EnumValidator?.Invoke(val);
                    info.Property.SetValue(target, val);
                }
            }
        }
    }
}
