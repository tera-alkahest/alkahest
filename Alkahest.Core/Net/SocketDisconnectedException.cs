using System;
using System.Runtime.Serialization;

namespace Alkahest.Core.Net
{
    [Serializable]
    public class SocketDisconnectedException : Exception
    {
        public SocketDisconnectedException()
            : base("Socket was disconnected normally by the remote peer.")
        {
        }

        public SocketDisconnectedException(string message)
            : base(message)
        {
        }

        public SocketDisconnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SocketDisconnectedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
