using Alkahest.Core.Logging;
using EasyHook;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;

namespace Alkahest.Scanner
{
    sealed class IpcChannel : MarshalByRefObject
    {
        public static string Create()
        {
            string name = null;

            RemoteHooking.IpcCreateServer<IpcChannel>(ref name, WellKnownObjectMode.Singleton);

            return name;
        }

        public static IpcChannel Connect(string name)
        {
            return RemoteHooking.IpcConnectClient<IpcChannel>(name);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        static readonly Log _log = new Log(typeof(IpcChannel));

        readonly ManualResetEventSlim _event = new ManualResetEventSlim();

        public uint? Version1 { get; set; }

        public uint? Version2 { get; set; }

        public IReadOnlyList<byte> DataCenterKey { get; set; }

        public IReadOnlyList<byte> DataCenterIV { get; set; }

        public IReadOnlyList<Tuple<ushort, string>> GameMessages { get; set; }

        public IReadOnlyList<Tuple<ushort, string>> SystemMessages { get; set; }

        public void LogBasic(string format, params object[] args)
        {
            _log.Basic(format, args);
        }

        public void LogError(string format, params object[] args)
        {
            _log.Error(format, args);
        }

        public void Done()
        {
            _event.Set();
        }

        public void Wait()
        {
            _event.Wait();
        }
    }
}
