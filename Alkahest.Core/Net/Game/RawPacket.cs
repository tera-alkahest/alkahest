using Alkahest.Core.Collections;
using System;
using System.Text;

namespace Alkahest.Core.Net.Game
{
    public sealed class RawPacket
    {
        public string Name { get; }

        public byte[] Payload { get; set; }

        public RawPacket(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string ToString()
        {
            var hex = new StringBuilder();

            foreach (var (i, x) in Payload.WithIndex())
            {
                hex.Append($"{x:X2}");

                if (i != Payload.Length - 1)
                {
                    if ((i + 1) % 16 != 0)
                        hex.Append(" ");
                    else
                        hex.AppendLine();
                }
            }

            var text = new StringBuilder();

            foreach (var (i, x) in Payload.WithIndex())
            {
                var ch = (char)x;

                text.Append(!char.IsControl(ch) && !char.IsWhiteSpace(ch) ? ch : '.');

                if (i != Payload.Length - 1 && (i + 1) % 16 == 0)
                    text.AppendLine();
            }

            var hexLines = hex.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var textLines = text.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var final = new StringBuilder();

            foreach (var (i, line) in hexLines.WithIndex())
            {
                final.Append($"{i * 16:X4}:  ");
                final.AppendFormat($"{line,-47}  ");
                final.Append(textLines[i]);

                if (i != hexLines.Length - 1)
                    final.AppendLine();
            }

            return final.ToString();
        }
    }
}
