using System;
using System.Collections.Generic;
using System.Diagnostics;
using EasyHook;
using Alkahest.Scanner.Scanners;

namespace Alkahest.Scanner
{
    public sealed class EntryPoint : IEntryPoint
    {
        static readonly IEnumerable<IScanner> _scanners = new IScanner[]
        {
            new ClientVersionScanner(),
            new DataCenterKeyScanner(),
            new GameMessageScanner(),
            new SystemMessageScanner(),
        };

        readonly IpcChannel _channel;

#pragma warning disable IDE0060 // Remove unused parameter
        public EntryPoint(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _channel = IpcChannel.Connect(channelName);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public void Run(RemoteHooking.IContext context, string channelName)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var mod = Process.GetCurrentProcess().MainModule;
            var reader = new MemoryReader(mod.BaseAddress,
                mod.ModuleMemorySize);

            foreach (var scanner in _scanners)
            {
                var name = scanner.GetType().Name;

                _channel.LogBasic("Running {0}...", name);

                try
                {
                    scanner.Run(reader, _channel);
                }
                catch (Exception e)
                {
                    _channel.LogError("Exception in {0}:", name);
                    _channel.LogError(e.ToString());
                }
            }

            _channel.Done();
        }
    }
}
