namespace Alkahest.Core.Net.Game
{
    public sealed class GameMessageTable : MessageTable
    {
        public GameMessageTable(uint version)
            : base(true, version)
        {
        }
    }
}
