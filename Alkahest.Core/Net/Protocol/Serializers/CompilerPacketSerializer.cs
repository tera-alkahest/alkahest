using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Protocol.OpCodes;
using Mono.Linq.Expressions;

namespace Alkahest.Core.Net.Protocol.Serializers
{
    public sealed class CompilerPacketSerializer : PacketSerializer
    {
        sealed class CompilerPacketFieldInfo : PacketFieldInfo
        {
            public CompilerPacketFieldInfo(PropertyInfo property,
                PacketFieldAttribute attribute)
                : base(property, attribute)
            {
            }
        }

        const string ReaderPositionName = nameof(TeraBinaryReader.Position);

        const string ReadName = "Read";

        const string ReadUInt16Name = nameof(TeraBinaryReader.ReadUInt16);

        const string ReadStringName = nameof(TeraBinaryReader.ReadString);

        const string ReadOffsetName = nameof(TeraBinaryReader.ReadOffset);

        const string ReadBytesName = nameof(TeraBinaryReader.ReadBytes);

        const string WriterPositionName = nameof(TeraBinaryWriter.Position);

        const string WriteName = "Write";

        const string WriteUInt16Name = nameof(TeraBinaryWriter.WriteUInt16);

        const string WriteStringName = nameof(TeraBinaryWriter.WriteString);

        const string WriteOffsetName = nameof(TeraBinaryWriter.WriteOffset);

        const string WriteBytesName = nameof(TeraBinaryWriter.WriteBytes);

        const string CountName = nameof(List<object>.Count);

        const string AddName = nameof(List<object>.Add);

        const string AddRangeName = nameof(List<object>.AddRange);

        const string ClearName = nameof(List<object>.Clear);

        const string ToArrayName = nameof(List<object>.ToArray);

        const string ItemName = "Item";

        const string LocalName = "Local";

        static readonly Log _log = new Log(typeof(CompilerPacketSerializer));

        readonly Dictionary<Type, Action<TeraBinaryReader, object>> _deserializers =
            new Dictionary<Type, Action<TeraBinaryReader, object>>();

        readonly Dictionary<Type, Action<TeraBinaryWriter, object>> _serializers =
            new Dictionary<Type, Action<TeraBinaryWriter, object>>();

        public CompilerPacketSerializer(MessageTables messages)
            : base(messages)
        {
            foreach (var opCode in messages.Game.OpCodeToName.Keys)
            {
                var type = GetType(opCode);

                if (type == null)
                    continue;

                _serializers.Add(type, CompileSerializer(type));
                _deserializers.Add(type, CompileDeserializer(type));
            }

            _log.Basic("Compiled {0} packet serializers", _serializers.Count);
        }

        protected override PacketFieldInfo CreateFieldInfo(
            PropertyInfo property, PacketFieldAttribute attribute)
        {
            return new CompilerPacketFieldInfo(property, attribute);
        }

        protected override void OnSerialize(TeraBinaryWriter writer, Packet packet)
        {
            _serializers[packet.GetType()](writer, packet);
        }

        Action<TeraBinaryWriter, object> CompileSerializer(Type type)
        {
            if (_serializers.TryGetValue(type, out var s))
                return s;

            var writer = Expression.Parameter(typeof(TeraBinaryWriter), "writer");
            var source = Expression.Parameter(typeof(object), "source");
            var packet = Expression.Variable(type, "packet2");

            var offsets = new List<(PacketFieldInfo info, ParameterExpression offset)>();

            BlockExpression CompileByteArray1(PacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = Expression.Variable(typeof(int), $"offset{prop.Name}");

                offsets.Add((info, offset));

                var property = packet.Property(prop);

                return Expression.Block(
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }),
                    writer.Call(WriteUInt16Name, null, new[] { property.Property(CountName).Convert(typeof(ushort)) }));
            }

            BlockExpression CompileArray1(PacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = Expression.Variable(typeof(int), $"offset{prop.Name}");

                offsets.Add((info, offset));

                var property = packet.Property(prop);

                return Expression.Block(
                    writer.Call(WriteUInt16Name, null, new[] { property.Property(CountName).Convert(typeof(ushort)) }),
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }));
            }

            BlockExpression CompileString1(PacketFieldInfo info)
            {
                var offset = Expression.Variable(typeof(int), $"offset{info.Property.Name}");

                offsets.Add((info, offset));

                return Expression.Block(
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }));
            }

            BlockExpression CompilePrimitive(PacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var etype = ftype.IsEnum ? ftype.GetEnumUnderlyingType() : ftype;
                var prefix = info.Attribute.IsLocalSkill ? LocalName : string.Empty;

                Expression property;

                if (ftype == typeof(ushort) && info.Attribute.IsUnknownArray)
                    property = ((ushort)0).Constant();
                else
                {
                    property = packet.Property(prop);

                    if (ftype.IsEnum)
                        property = property.Convert(etype);
                }

                return Expression.Block(
                    writer.Call(WriteName + prefix + etype.Name, null, new[] { property }));
            }

            var exprs = new List<Expression>()
            {
                packet.Assign(source.Convert(type)),
            };

            foreach (var info in GetPacketFields<PacketFieldInfo>(type))
                exprs.Add(
                    info.IsByteArray ? CompileByteArray1(info) :
                    info.IsArray ? CompileArray1(info) :
                    info.IsString ? CompileString1(info) :
                    CompilePrimitive(info));

            BlockExpression CompileByteArray2(PacketFieldInfo info,
                ParameterExpression offset)
            {
                var property = Expression.Variable(info.Property.PropertyType, "property");
                var position = Expression.Variable(typeof(int), "position");

                var write = Expression.Block(new[] { position },
                    position.Assign(writer.Property(WriterPositionName)),
                    writer.Property(WriterPositionName).Assign(offset),
                    writer.Call(WriteOffsetName, null, new[] { position }),
                    writer.Property(WriterPositionName).Assign(position),
                    writer.Call(WriteBytesName, null, new[] { property.Call(ToArrayName, null, null) }));

                return Expression.Block(new[] { property },
                    property.Assign(packet.Property(info.Property)),
                    property.Property(CountName).NotEqual(0.Constant())
                        .IfThen(write));
            }

            BlockExpression CompileArray2(PacketFieldInfo info,
                ParameterExpression offset)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var elemType = ftype.GetGenericArguments()[0];

                var property = Expression.Variable(ftype, "property");
                var count = Expression.Variable(typeof(int), "count");
                var position = Expression.Variable(typeof(int), "position");
                var i = Expression.Variable(typeof(int), "i");
                var position2 = Expression.Variable(typeof(int), "position2");
                var position3 = Expression.Variable(typeof(int), "position3");

                var writeNext = Expression.Block(new[] { position3 },
                    position3.Assign(writer.Property(WriterPositionName)),
                    writer.Property(WriterPositionName).Assign(position2.Add(sizeof(ushort).Constant())),
                    writer.Call(WriteOffsetName, null, new[] { position3 }),
                    writer.Property(WriterPositionName).Assign(position3));

                var loop = CustomExpression.For(i, 0.Constant(),
                    i.LessThan(count), i.PostIncrementAssign(),
                    Expression.Block(new[] { position2 },
                        position2.Assign(writer.Property(WriterPositionName)),
                        writer.Call(WriteOffsetName, null, new[] { position2 }),
                        writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }),
                        CompileSerializer(elemType).Constant().Invoke(writer, property.Property(ItemName, i)),
                        i.NotEqual(count.Subtract(1.Constant()))
                            .IfThen(writeNext)));

                var write = Expression.Block(new[] { position, i },
                    position.Assign(writer.Property(WriterPositionName)),
                    writer.Property(WriterPositionName).Assign(offset),
                    writer.Call(WriteOffsetName, null, new[] { position }),
                    writer.Property(WriterPositionName).Assign(position),
                    loop);

                return Expression.Block(new[] { property, count },
                    property.Assign(packet.Property(prop)),
                    count.Assign(property.Property(CountName)),
                    count.NotEqual(0.Constant())
                        .IfThen(write));
            }

            BlockExpression CompileString2(PacketFieldInfo info,
                ParameterExpression offset)
            {
                var position = Expression.Variable(typeof(int), "position");

                return Expression.Block(new[] { position },
                    position.Assign(writer.Property(WriterPositionName)),
                    writer.Property(WriterPositionName).Assign(offset),
                    writer.Call(WriteOffsetName, null, new[] { position }),
                    writer.Property(WriterPositionName).Assign(position),
                    writer.Call(WriteStringName, null, new[] { packet.Property(info.Property) }));
            }

            foreach (var (info, offset) in offsets)
                exprs.Add(
                    info.IsByteArray ? CompileByteArray2(info, offset) :
                    info.IsArray ? CompileArray2(info, offset) :
                    CompileString2(info, offset));

            var vars = new[] { packet }.Concat(offsets.Select(tup => tup.offset));

            return Expression.Lambda<Action<TeraBinaryWriter, object>>(
                Expression.Block(vars, exprs), writer, source).Compile();
        }

        protected override void OnDeserialize(TeraBinaryReader reader,
            Packet packet)
        {
            _deserializers[packet.GetType()](reader, packet);
        }

        Action<TeraBinaryReader, object> CompileDeserializer(Type type)
        {
            if (_deserializers.TryGetValue(type, out var d))
                return d;

            var reader = Expression.Parameter(typeof(TeraBinaryReader), "reader");
            var target = Expression.Parameter(typeof(object), "target");
            var packet = Expression.Variable(type, "packet");

            BlockExpression CompileByteArray(PacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = Expression.Variable(typeof(int), "offset");
                var count = Expression.Variable(typeof(ushort), "count");
                var property = Expression.Variable(prop.PropertyType, "property");
                var position = Expression.Variable(typeof(int), "position");

                var read = Expression.Block(new[] { position },
                    position.Assign(reader.Property(ReaderPositionName)),
                    reader.Property(ReaderPositionName).Assign(offset),
                    property.Call(AddRangeName, null, new[] { reader.Call(ReadBytesName, null, new[] { count.Convert(typeof(int)) }) }),
                    reader.Property(ReaderPositionName).Assign(position));

                return Expression.Block(new[] { offset, count, property },
                    offset.Assign(reader.Call(ReadOffsetName, null, null)),
                    count.Assign(reader.Call(ReadUInt16Name, null, null)),
                    property.Assign(packet.Property(prop)),
                    property.Call(ClearName, null, null),
                    count.NotEqual(((ushort)0).Constant())
                        .IfThen(read));
            }

            BlockExpression CompileArray(PacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var elemType = ftype.GetGenericArguments()[0];

                var offset = Expression.Variable(typeof(int), "offset");
                var count = Expression.Variable(typeof(ushort), "count");
                var position = Expression.Variable(typeof(int), "position");
                var property = Expression.Variable(ftype, "property");
                var next = Expression.Variable(typeof(int), "next");
                var i = Expression.Variable(typeof(int), "i");
                var elem = Expression.Variable(elemType, "elem");

                var loop = CustomExpression.For(i, 0.Constant(),
                    i.LessThan(count.Convert(typeof(int))),
                    i.PostIncrementAssign(),
                    Expression.Block(
                        reader.Property(ReaderPositionName).Assign(next),
                        reader.Call(ReadOffsetName, null, null),
                        next.Assign(reader.Call(ReadOffsetName, null, null)),
                        elem.Assign(elemType.New()),
                        CompileDeserializer(elemType).Constant().Invoke(reader, elem),
                        property.Call(AddName, null, new[] { elem })));

                var read = Expression.Block(new[] { position, next, elem },
                    position.Assign(reader.Property(ReaderPositionName)),
                    next.Assign(offset),
                    loop,
                    reader.Property(ReaderPositionName).Assign(position));

                return Expression.Block(new[] { offset, count, property },
                    count.Assign(reader.Call(ReadUInt16Name, null, null)),
                    offset.Assign(reader.Call(ReadOffsetName, null, null)),
                    property.Assign(packet.Property(prop)),
                    property.Call(ClearName, null, null),
                    count.NotEqual(((ushort)0).Constant())
                        .IfThen(read));
            }

            BlockExpression CompileString(PacketFieldInfo info)
            {
                var offset = Expression.Variable(typeof(int), "offset");
                var position = Expression.Variable(typeof(int), "position");

                return Expression.Block(new[] { offset, position },
                    offset.Assign(reader.Call(ReadOffsetName, null, null)),
                    position.Assign(reader.Property(ReaderPositionName)),
                    reader.Property(ReaderPositionName).Assign(offset),
                    packet.Property(info.Property).Assign(reader.Call(ReadStringName, null, null)),
                    reader.Property(ReaderPositionName).Assign(position));
            }

            BlockExpression CompilePrimitive(PacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var etype = ftype.IsEnum ? ftype.GetEnumUnderlyingType() : ftype;
                var prefix = info.Attribute.IsLocalSkill ? LocalName : string.Empty;

                Expression read = reader.Call(ReadName + prefix + etype.Name, null, null);

                if (ftype.IsEnum)
                    read = read.Convert(ftype);

                return Expression.Block(packet.Property(prop).Assign(read));
            }

            var exprs = new List<Expression>()
            {
                packet.Assign(target.Convert(type)),
            };

            foreach (var info in GetPacketFields<PacketFieldInfo>(type))
                exprs.Add(
                    info.IsByteArray ? CompileByteArray(info) :
                    info.IsArray ? CompileArray(info) :
                    info.IsString ? CompileString(info) :
                    CompilePrimitive(info));

            return Expression.Lambda<Action<TeraBinaryReader, object>>(
                Expression.Block(new[] { packet }, exprs), reader, target).Compile();
        }
    }
}
