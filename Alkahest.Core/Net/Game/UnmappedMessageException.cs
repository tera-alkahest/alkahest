using System;
using System.Runtime.Serialization;

namespace Alkahest.Core.Net.Game
{
    [Serializable]
    public class UnmappedMessageException : Exception
    {
        public UnmappedMessageException()
            : this("Game message name is not mapped to a code.")
        {
        }

        public UnmappedMessageException(string message)
            : base(message)
        {
        }

        public UnmappedMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnmappedMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
