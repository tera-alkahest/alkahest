namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public sealed class GameMessageTable : OpCodeTable
    {
        public GameMessageTable(int version)
            : base(true, version)
        {
        }
    }
}
