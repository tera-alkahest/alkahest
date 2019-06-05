using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Alkahest.Scanner
{
    public sealed class EntryPoint : IEntryPoint
    {
        static readonly IEnumerable<IScanner> _scanners = new IScanner[]
        {
            new ClientVersionScanner(),
            new DataCenterScanner(),
            new GameMessageScanner(),
            new SystemMessageScanner(),
        };

        readonly IpcChannel _channel;

#pragma warning disable IDE0060 // Remove unused parameter
        public EntryPoint(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _channel = IpcChannel.Connect(channelName);

            _channel.LogBasic("Injection successful");
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void Run(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            try
            {
                var mod = Process.GetCurrentProcess().MainModule;
                var reader = new MemoryReader(mod.BaseAddress, mod.ModuleMemorySize);

                foreach (var scanner in _scanners)
                {
                    _channel.LogBasic("Running {0}...", scanner.GetType().Name);

                    scanner.Run(reader, _channel);
                }

                _channel.Signal();
            }
            catch (Exception ex)
            {
                _channel.LogError("{0}", ex);
            }
        }
    }
}
