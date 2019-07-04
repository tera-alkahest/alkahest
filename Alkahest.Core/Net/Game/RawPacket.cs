using Alkahest.Core.Collections;
using System;
using System.Text;

namespace Alkahest.Core.Net.Game
{
    public sealed class RawPacket
    {
        public string Name { get; }

        public Memory<byte> Payload { get; set; }

        public RawPacket(string name)
        {
            Name = name;
        }

        internal static void ToString(StringBuilder builder, ReadOnlyMemory<byte> data, string indent)
        {
            var bytes = data.ToEnumerable().WithIndex();
            var hex = new StringBuilder();
            var length = data.Length;

            foreach (var (i, x) in bytes)
            {
                hex.Append($"{x:X2}");

                if (i != length - 1)
                {
                    if ((i + 1) % 16 != 0)
                        hex.Append(" ");
                    else
                        hex.AppendLine();
                }
            }

            var text = new StringBuilder();

            foreach (var (i, x) in bytes)
            {
                var ch = (char)x;

                text.Append(!char.IsControl(ch) && !char.IsWhiteSpace(ch) ? ch : '.');

                if (i != length - 1 && (i + 1) % 16 == 0)
                    text.AppendLine();
            }

            var hexLines = hex.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
            var textLines = text.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var (i, line) in hexLines.WithIndex())
            {
                builder.Append(indent);
                builder.Append($"{i * 16:X4}:  ");
                builder.AppendFormat($"{line,-47}  ");
                builder.Append(textLines[i]);

                if (i != hexLines.Length - 1)
                    builder.AppendLine();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            ToString(sb, Payload, string.Empty);

            return sb.ToString();
        }
    }
}
