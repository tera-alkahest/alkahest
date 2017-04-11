namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public sealed class MessageTables
    {
        public GameMessageTable Game { get; set; }

        public SystemMessageTable System { get; set; }

        public MessageTables(int version)
        {
            Game = new GameMessageTable(version);
            System = new SystemMessageTable(version);
        }
    }
}
