using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Mono.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Alkahest.Core.Net.Game.Serialization
{
    public sealed class CompilerPacketSerializer : PacketSerializer
    {
        sealed class CompilerPacketFieldInfo : PacketFieldInfo
        {
            public CompilerPacketFieldInfo(PropertyInfo property,
                PacketFieldOptionsAttribute attribute)
                : base(property, attribute)
            {
            }
        }

        const string ReaderPositionName = nameof(GameBinaryReader.Position);

        const string ReadName = "Read";

        const string ReadUInt16Name = nameof(GameBinaryReader.ReadUInt16);

        const string ReadStringName = nameof(GameBinaryReader.ReadString);

        const string ReadOffsetName = nameof(GameBinaryReader.ReadOffset);

        const string ReadBytesName = nameof(GameBinaryReader.ReadBytes);

        const string WriterPositionName = nameof(GameBinaryWriter.Position);

        const string WriteName = "Write";

        const string WriteUInt16Name = nameof(GameBinaryWriter.WriteUInt16);

        const string WriteStringName = nameof(GameBinaryWriter.WriteString);

        const string WriteOffsetName = nameof(GameBinaryWriter.WriteOffset);

        const string WriteBytesName = nameof(GameBinaryWriter.WriteBytes);

        const string CountName = nameof(List<object>.Count);

        const string AddName = nameof(List<object>.Add);

        const string AddRangeName = nameof(List<object>.AddRange);

        const string ClearName = nameof(List<object>.Clear);

        const string ToArrayName = nameof(List<object>.ToArray);

        const string ItemName = "Item";

        const string SimpleName = "Simple";

        static readonly Log _log = new Log(typeof(CompilerPacketSerializer));

        readonly Dictionary<PacketInfo, Func<SerializablePacket>> _creators =
            new Dictionary<PacketInfo, Func<SerializablePacket>>();

        readonly Dictionary<PacketInfo, Action<GameBinaryReader, object>> _deserializers =
            new Dictionary<PacketInfo, Action<GameBinaryReader, object>>();

        readonly Dictionary<PacketInfo, Action<GameBinaryWriter, object>> _serializers =
            new Dictionary<PacketInfo, Action<GameBinaryWriter, object>>();

        public CompilerPacketSerializer(Region region, GameMessageTable gameMessages,
            SystemMessageTable systemMessages)
            : base(region, gameMessages, systemMessages)
        {
            foreach (var code in gameMessages.CodeToName.Keys)
            {
                var info = GetPacketInfo(code);

                if (info == null)
                    continue;

                _creators.Add(info, CompileCreator(info));
                _serializers.Add(info, CompileSerializer(info));
                _deserializers.Add(info, CompileDeserializer(info));
            }

            _log.Basic("Compiled {0} packet serializers", _serializers.Count);
        }

        protected override PacketFieldInfo CreateFieldInfo(PropertyInfo property,
            PacketFieldOptionsAttribute attribute)
        {
            return new CompilerPacketFieldInfo(property, attribute);
        }

        protected override SerializablePacket OnCreate(PacketInfo info)
        {
            return _creators[info]();
        }

        Func<SerializablePacket> CompileCreator(PacketInfo info)
        {
            return _creators.TryGetValue(info, out var c) ? c :
                Expression.Lambda<Func<SerializablePacket>>(info.Type.New()).Compile();
        }

        protected override void OnSerialize(GameBinaryWriter writer, PacketInfo info,
            SerializablePacket packet)
        {
            _serializers[info](writer, packet);
        }

        Action<GameBinaryWriter, object> CompileSerializer(PacketInfo info)
        {
            if (_serializers.TryGetValue(info, out var s))
                return s;

            var writer = typeof(GameBinaryWriter).Parameter("writer");
            var source = typeof(object).Parameter("source");
            var packet = info.Type.Variable("packet2");

            var offsets = new List<(CompilerPacketFieldInfo, ParameterExpression)>();

            BlockExpression CompileByteArray1(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = typeof(int).Variable($"offset{prop.Name}");

                offsets.Add((info, offset));

                var property = packet.Property(prop);

                return Expression.Block(
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }),
                    writer.Call(WriteUInt16Name, null, new[] { property.Property(CountName).Convert(typeof(ushort)) }));
            }

            BlockExpression CompileArray1(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = typeof(int).Variable($"offset{prop.Name}");

                offsets.Add((info, offset));

                var property = packet.Property(prop);

                return Expression.Block(
                    writer.Call(WriteUInt16Name, null, new[] { property.Property(CountName).Convert(typeof(ushort)) }),
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }));
            }

            BlockExpression CompileString1(CompilerPacketFieldInfo info)
            {
                var offset = typeof(int).Variable($"offset{info.Property.Name}");

                offsets.Add((info, offset));

                return Expression.Block(
                    offset.Assign(writer.Property(WriterPositionName)),
                    writer.Call(WriteUInt16Name, null, new[] { ((ushort)0).Constant() }));
            }

            BlockExpression CompilePrimitive(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var etype = ftype.IsEnum ? ftype.GetEnumUnderlyingType() : ftype;
                var prefix = (info.Attribute?.IsSimpleSkill ?? false) ? SimpleName : string.Empty;

                Expression property;

                if (ftype == typeof(ushort) && (info.Attribute?.IsUnknownArray ?? false))
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
                packet.Assign(source.Convert(info.Type)),
            };

            foreach (var field in info.Fields.Cast<CompilerPacketFieldInfo>())
                exprs.Add(
                    field.IsByteArray ? CompileByteArray1(field) :
                    field.IsArray ? CompileArray1(field) :
                    field.IsString ? CompileString1(field) :
                    CompilePrimitive(field));

            BlockExpression CompileByteArray2(CompilerPacketFieldInfo info, ParameterExpression offset)
            {
                var property = info.Property.PropertyType.Variable("property");
                var position = typeof(int).Variable("position");

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

            BlockExpression CompileArray2(CompilerPacketFieldInfo info, ParameterExpression offset)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var elemInfo = GetPacketInfo(ftype.GetGenericArguments()[0]);

                var property = ftype.Variable("property");
                var count = typeof(int).Variable("count");
                var position = typeof(int).Variable("position");
                var i = typeof(int).Variable("i");
                var position2 = typeof(int).Variable("position2");
                var position3 = typeof(int).Variable("position3");

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
                        CompileSerializer(elemInfo).Constant().Invoke(writer, property.Property(ItemName, i)),
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

            BlockExpression CompileString2(CompilerPacketFieldInfo info, ParameterExpression offset)
            {
                var position = typeof(int).Variable("position");

                return Expression.Block(new[] { position },
                    position.Assign(writer.Property(WriterPositionName)),
                    writer.Property(WriterPositionName).Assign(offset),
                    writer.Call(WriteOffsetName, null, new[] { position }),
                    writer.Property(WriterPositionName).Assign(position),
                    writer.Call(WriteStringName, null, new[] { packet.Property(info.Property) }));
            }

            foreach (var (field, offset) in offsets)
                exprs.Add(
                    field.IsByteArray ? CompileByteArray2(field, offset) :
                    field.IsArray ? CompileArray2(field, offset) :
                    CompileString2(field, offset));

            var vars = new[] { packet }.Concat(offsets.Select(tup => tup.Item2));

            return Expression.Lambda<Action<GameBinaryWriter, object>>(
                Expression.Block(vars, exprs), writer, source).Compile();
        }

        protected override void OnDeserialize(GameBinaryReader reader, PacketInfo info,
            SerializablePacket packet)
        {
            _deserializers[info](reader, packet);
        }

        Action<GameBinaryReader, object> CompileDeserializer(PacketInfo info)
        {
            if (_deserializers.TryGetValue(info, out var d))
                return d;

            var reader = typeof(GameBinaryReader).Parameter("reader");
            var target = typeof(object).Parameter("target");
            var packet = info.Type.Variable("packet");

            BlockExpression CompileByteArray(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;

                var offset = typeof(int).Variable("offset");
                var count = typeof(ushort).Variable("count");
                var property = prop.PropertyType.Variable("property");
                var position = typeof(int).Variable("position");

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

            BlockExpression CompileArray(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var elemInfo = GetPacketInfo(ftype.GetGenericArguments()[0]);

                var offset = typeof(int).Variable("offset");
                var count = typeof(ushort).Variable("count");
                var position = typeof(int).Variable("position");
                var property = ftype.Variable("property");
                var next = typeof(int).Variable("next");
                var i = typeof(int).Variable("i");
                var elem = elemInfo.Type.Variable("elem");

                var loop = CustomExpression.For(i, 0.Constant(),
                    i.LessThan(count.Convert(typeof(int))),
                    i.PostIncrementAssign(),
                    Expression.Block(
                        reader.Property(ReaderPositionName).Assign(next),
                        reader.Call(ReadOffsetName, null, null),
                        next.Assign(reader.Call(ReadOffsetName, null, null)),
                        elem.Assign(elemInfo.Type.New()),
                        CompileDeserializer(elemInfo).Constant().Invoke(reader, elem),
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

            BlockExpression CompileString(CompilerPacketFieldInfo info)
            {
                var offset = typeof(int).Variable("offset");
                var position = typeof(int).Variable("position");

                return Expression.Block(new[] { offset, position },
                    offset.Assign(reader.Call(ReadOffsetName, null, null)),
                    position.Assign(reader.Property(ReaderPositionName)),
                    reader.Property(ReaderPositionName).Assign(offset),
                    packet.Property(info.Property).Assign(reader.Call(ReadStringName, null, null)),
                    reader.Property(ReaderPositionName).Assign(position));
            }

            BlockExpression CompilePrimitive(CompilerPacketFieldInfo info)
            {
                var prop = info.Property;
                var ftype = prop.PropertyType;
                var etype = ftype.IsEnum ? ftype.GetEnumUnderlyingType() : ftype;
                var prefix = (info.Attribute?.IsSimpleSkill ?? false) ? SimpleName : string.Empty;

                Expression read = reader.Call(ReadName + prefix + etype.Name, null, null);

                if (ftype.IsEnum)
                    read = read.Convert(ftype);

                return Expression.Block(packet.Property(prop).Assign(read));
            }

            var exprs = new List<Expression>()
            {
                packet.Assign(target.Convert(info.Type)),
            };

            foreach (var field in info.Fields.Cast<CompilerPacketFieldInfo>())
                exprs.Add(
                    field.IsByteArray ? CompileByteArray(field) :
                    field.IsArray ? CompileArray(field) :
                    field.IsString ? CompileString(field) :
                    CompilePrimitive(field));

            return Expression.Lambda<Action<GameBinaryReader, object>>(
                Expression.Block(new[] { packet }, exprs), reader, target).Compile();
        }
    }
}
