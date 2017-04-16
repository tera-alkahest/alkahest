namespace Alkahest.Core.Net.Protocol.OpCodes
{
    public sealed class MessageTables
    {
        public Region Region { get; }

        public GameMessageTable Game { get; }

        public SystemMessageTable System { get; }

        public MessageTables(Region region, int version)
        {
            region.CheckValidity(nameof(region));

            Region = region;
            Game = new GameMessageTable(version);
            System = new SystemMessageTable(version);
        }
    }
}
