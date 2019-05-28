using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSystemMessagePacket : Packet
    {
        const string Name = "S_SYSTEM_MESSAGE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSystemMessagePacket();
        }

        [PacketField]
        public string Message { get; set; }

        public string MessageName { get; set; }

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

            foreach (var kvp in MessageArguments)
                sb.Append($"\v{kvp.Key}\v{kvp.Value}");

            Message = sb.ToString();
        }
    }
}
