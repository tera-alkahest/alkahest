using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Alkahest.Core.Net.Game.Serialization
{
    public abstract class SerializablePacket
    {
        static readonly ConcurrentDictionary<Type, string> _names =
            new ConcurrentDictionary<Type, string>();

        [PacketFieldOptions(Skip = true)]
        public string Name { get; }

        internal SerializablePacket()
        {
            Name = _names.GetOrAdd(GetType(), t => t.GetCustomAttribute<PacketAttribute>().Name);
        }

        internal virtual void OnSerialize(PacketSerializer serializer)
        {
        }

        internal virtual void OnDeserialize(PacketSerializer serializer)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            ToString(sb, this, null, string.Empty, 0);

            return sb.ToString();
        }

        static void ToString(StringBuilder builder, object source, string header, string indent, int level)
        {
            var type = source.GetType();

            if (header == null)
                header = $"{type.Name} =";

            builder.AppendLine($"{indent}{header}");
            builder.AppendLine($"{indent}{{");

            foreach (var prop in type.GetProperties().OrderBy(p => p.MetadataToken))
            {
                var opts = prop.GetCustomAttribute<PacketFieldOptionsAttribute>();

                if (opts != null && opts.Skip)
                    continue;

                var name = prop.Name;
                var value = prop.GetValue(source);

                if (value is IList list)
                {
                    builder.AppendLine($"{indent}  {name} = [{list.Count}]");
                    builder.AppendLine($"{indent}  {{");

                    var elemIndent = indent + "    ";

                    if (value is List<byte> bytes)
                    {
                        RawPacket.ToString(builder, bytes.ToArray(), elemIndent);
                        builder.AppendLine();
                    }
                    else
                        for (var i = 0; i < list.Count; i++)
                            ToString(builder, list[i], $"[{i}] =", elemIndent, level + 1);

                    builder.AppendLine($"{indent}  }}");
                }
                else if (value is string str)
                    builder.AppendLine($"{indent}  {name} = \"{str}\"");
                else
                    builder.AppendLine($"{indent}  {name} = {value}");
            }

            var end = $"{indent}}}";

            if (level == 0)
                builder.Append(end);
            else
                builder.AppendLine(end);
        }
    }
}
