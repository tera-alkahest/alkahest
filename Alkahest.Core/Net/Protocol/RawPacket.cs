using System;
using System.Text;

namespace Alkahest.Core.Net.Protocol
{
    public sealed class RawPacket
    {
        public string OpCode { get; }

        public byte[] Payload { get; set; }

        public RawPacket(string opCode)
        {
            OpCode = opCode;
        }

        public override string ToString()
        {
            var hex = new StringBuilder();

            for (var i = 0; i < Payload.Length; i++)
            {
                hex.Append($"{Payload[i]:X2}");

                if (i != Payload.Length - 1)
                {
                    if ((i + 1) % 16 != 0)
                        hex.Append(" ");
                    else
                        hex.AppendLine();
                }
            }

            var text = new StringBuilder();

            for (var i = 0; i < Payload.Length; i++)
            {
                var ch = (char)Payload[i];

                text.Append(!char.IsControl(ch) && !char.IsWhiteSpace(ch) ?
                    ch : '.');

                if (i != Payload.Length - 1 && (i + 1) % 16 == 0)
                    text.AppendLine();
            }

            var hexLines = hex.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var textLines = text.ToString().Split(new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            var final = new StringBuilder();

            for (var i = 0; i < hexLines.Length; i++)
            {
                final.Append($"{i * 16:X4}:  ");
                final.AppendFormat($"{hexLines[i],-47}  ");
                final.Append(textLines[i]);

                if (i != hexLines.Length - 1)
                    final.AppendLine();
            }

            return final.ToString();
        }
    }
}
