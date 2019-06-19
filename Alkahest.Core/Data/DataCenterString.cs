namespace Alkahest.Core.Data
{
    sealed class DataCenterString
    {
        public uint Index { get; }

        public DataCenterAddress Address { get; }

        public string Value { get; }

        public uint Hash { get; }

        public DataCenterString(uint index, DataCenterAddress address, string value, uint hash)
        {
            Index = index;
            Address = address;
            Value = value;
            Hash = hash;
        }
    }
}
