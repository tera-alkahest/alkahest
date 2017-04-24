namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public sealed class GameMessageTable : OpCodeTable
    {
        public GameMessageTable(uint version)
            : base(true, version)
        {
        }
    }
}
