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
    }
}
