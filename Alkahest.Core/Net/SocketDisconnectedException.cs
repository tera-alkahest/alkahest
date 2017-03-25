using System;

namespace Alkahest.Core.Net
{
    public class SocketDisconnectedException : Exception
    {
        public SocketDisconnectedException()
            : base("Socket was disconnected normally by the remote peer.")
        {
        }
    }
}
