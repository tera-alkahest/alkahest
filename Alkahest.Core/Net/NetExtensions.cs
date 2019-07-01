using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace Alkahest.Core.Net
{
    public static class NetExtensions
    {
        public static string ToErrorString(this SocketError error)
        {
            return new Win32Exception((int)error.CheckValidity(nameof(error))).Message;
        }

        public static void SendFull(this Socket socket, ArraySegment<byte> segment)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (segment.Array == null)
                throw new ArgumentException("Invalid array segment.", nameof(segment));

            var progress = 0;

            while (progress < segment.Count)
            {
                var len = socket.Send(segment.Array, segment.Offset + progress,
                    segment.Count - progress, SocketFlags.None);

                if (len == 0)
                    throw new SocketDisconnectedException();

                progress += len;
            }
        }

        public static void ReceiveFull(this Socket socket, ArraySegment<byte> segment)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (segment.Array == null)
                throw new ArgumentException("Invalid array segment.", nameof(segment));

            var progress = 0;

            while (progress < segment.Count)
            {
                var len = socket.Receive(segment.Array, segment.Offset + progress,
                    segment.Count - progress, SocketFlags.None);

                if (len == 0)
                    throw new SocketDisconnectedException();

                progress += len;
            }
        }

        public static void SafeClose(this Socket socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }

            socket.Dispose();
        }

        public static void Reset(this SocketAsyncEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            args.AcceptSocket = null;
            args.BufferList = null;
            args.DisconnectReuseSocket = false;
            args.RemoteEndPoint = null;
            args.SendPacketsElements = null;
            args.SendPacketsFlags = 0;
            args.SendPacketsSendSize = 0;
            args.SocketError = 0;
            args.SocketFlags = 0;
            args.UserToken = null;

            args.SetBuffer(null, 0, 0);
        }
    }
}
