using Alkahest.Core.Collections;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SYSTEM_MESSAGE")]
    public sealed class SSystemMessagePacket : SerializablePacket
    {
        public string Message { get; set; }

        [PacketFieldOptions(Skip = true)]
        public string MessageName { get; set; }

        [PacketFieldOptions(Skip = true)]
        public Dictionary<string, string> MessageArguments { get; } = new Dictionary<string, string>();

        internal override void OnDeserialize(PacketSerializer serializer)
        {
            var msg = Message.Split('\v');

            MessageName = serializer.SystemMessages.CodeToName[ushort.Parse(msg[0].Substring(1))];

            msg = msg.Skip(1).ToArray();

            var keys = msg.Where((_, i) => i % 2 == 0);
            var vals = msg.Where((_, i) => i % 2 != 0);

            MessageArguments.Clear();

            foreach (var (k, v) in keys.Zip(vals, (k, v) => (k, v)))
                MessageArguments.Add(k, v);
        }

        internal override void OnSerialize(PacketSerializer serializer)
        {
            var sb = new StringBuilder();

            sb.Append($"@{serializer.SystemMessages.NameToCode[MessageName]}");

            foreach (var (name, value) in MessageArguments.Tuples())
                sb.Append($"\v{name}\v{value}");

            Message = sb.ToString();
        }
    }
}
