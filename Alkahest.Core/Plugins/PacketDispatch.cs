using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Alkahest.Core.Plugins
{
    public sealed class PacketDispatch
    {
        static readonly Log _log = new Log(typeof(PacketDispatch));

        public PluginContext Context { get; }

        readonly List<PacketHandler> _globalHandlers = new List<PacketHandler>();

        readonly Dictionary<ushort, List<PacketHandler>> _codeHandlers =
            new Dictionary<ushort, List<PacketHandler>>();

        readonly object _listLock = new object();

        readonly object _invokeLock = new object();

        internal PacketDispatch(PluginContext context)
        {
            Context = context;

            foreach (var code in context.Serializer.GameMessages.CodeToName.Keys)
                _codeHandlers.Add(code, new List<PacketHandler>());
        }

        internal void Start()
        {
            foreach (var proxy in Context.Proxies)
            {
                proxy.PacketReceived += HandleReceive;
                proxy.PacketSent += HandleSend;
            }
        }

        internal void Stop()
        {
            foreach (var proxy in Context.Proxies)
            {
                proxy.PacketReceived -= HandleReceive;
                proxy.PacketSent -= HandleSend;
            }

            // Clean up after plugins that don't do it themselves.
            lock (_listLock)
            {
                _globalHandlers.Clear();

                foreach (var list in _codeHandlers.Values)
                    list.Clear();
            }
        }

        public PacketHandler AddHandler(GlobalPacketHandler handler, PacketFilter filter = null)
        {
            var ph = new PacketHandler(true, null, null, handler, filter);

            lock (_listLock)
                _globalHandlers.Add(ph);

            return ph;
        }

        public PacketHandler AddHandler(string name, RawPacketHandler handler, PacketFilter filter = null)
        {
            if (!Context.Serializer.GameMessages.NameToCode.TryGetValue(name, out var code))
                throw new UnmappedMessageException();

            var ph = new PacketHandler(true, name, code, handler, filter);
            var list = _codeHandlers[code];

            lock (_listLock)
                list.Add(ph);

            return ph;
        }

        public PacketHandler AddHandler<T>(TypedPacketHandler<T> handler, PacketFilter filter = null)
            where T : SerializablePacket
        {
            var name = typeof(T).GetCustomAttribute<PacketAttribute>()?.Name;

            if (name == null)
                throw new ArgumentException("Invalid packet type.");

            if (!Context.Serializer.GameMessages.NameToCode.TryGetValue(name, out var code))
                throw new UnmappedMessageException();

            var ph = new PacketHandler(false, name, code, handler, filter);
            var list = _codeHandlers[code];

            lock (_listLock)
                list.Add(ph);

            return ph;
        }

        public bool RemoveHandler(PacketHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var list = handler.Name == null ? _globalHandlers : _codeHandlers[(ushort)handler.Code];

            lock (_listLock)
                return list.Remove(handler);
        }

        void HandleReceive(object sender, ref PacketEventArgs args)
        {
            Handle(ref args, false);
        }

        void HandleSend(object sender, ref PacketEventArgs args)
        {
            Handle(ref args, true);
        }

        void Handle(ref PacketEventArgs args, bool forged)
        {
            var codeHandlers = _codeHandlers[args.Code];

            // There is obviously a race condition here, but that's OK; we want
            // to avoid allocations below if there are no relevant handlers.
            if (_globalHandlers.Count + codeHandlers.Count == 0)
                return;

            var handlers = new List<PacketHandler>();

            lock (_listLock)
            {
                handlers.AddRange(_globalHandlers);
                handlers.AddRange(codeHandlers);
            }

            handlers.Sort((a, b) => a.Filter.Order.CompareTo(b.Filter.Order));

            var serializer = Context.Serializer;
            var payload = args.Payload;

            serializer.GameMessages.CodeToName.TryGetValue(args.Code, out var name);

            var modified = false;
            var silenced = false;

            foreach (var handler in handlers)
            {
                var filter = handler.Filter;

                if (filter.Forged != null && filter.Forged != forged)
                    continue;

                if (filter.Modified != null && filter.Modified != modified)
                    continue;

                if (filter.Silenced != null && filter.Silenced != silenced)
                    continue;

                var flags = PacketFlags.None;

                if (forged)
                    flags |= PacketFlags.Forged;

                if (modified)
                    flags |= PacketFlags.Modified;

                if (silenced)
                    flags |= PacketFlags.Silenced;

                var newPayload = payload;

                if (handler.IsRaw)
                {
                    var global = handler.Name == null;

                    try
                    {
                        var packet = new RawPacket(name)
                        {
                            Payload = payload.ToArray(),
                        };

                        // Avoid DynamicInvoke so we don't box unnecessarily.
                        if (global)
                        {
                            var dg = (GlobalPacketHandler)handler.Handler;

                            lock (_invokeLock)
                                silenced = !dg(args.Client, args.Direction, args.Code, packet, flags);
                        }
                        else
                        {
                            var dg = (RawPacketHandler)handler.Handler;

                            lock (_invokeLock)
                                silenced = !dg(args.Client, args.Direction, packet, flags);
                        }

                        newPayload = packet.Payload;
                    }
                    catch (Exception ex) when (!Debugger.IsAttached)
                    {
                        _log.Error("Unhandled exception in {0} packet handler:",
                            global ? "global" : "raw");
                        _log.Error("{0}", ex);
                    }
                }
                else
                {
                    var packet = serializer.Create(name);
                    var success = true;

                    try
                    {
                        var seg = payload.GetArray();

                        serializer.Deserialize(seg.Array, seg.Offset, seg.Count, packet);
                    }
                    catch (EndOfStreamException)
                    {
                        _log.Error("{0}: {1} failed to deserialize; skipping typed packet handlers",
                            args.Direction.ToDirectionString(), name);

                        success = false;
                    }

                    if (success)
                    {
                        try
                        {
                            lock (_invokeLock)
                                silenced = (bool)handler.Handler.DynamicInvoke(args.Client,
                                    args.Direction, packet, flags);
                        }
                        catch (Exception ex) when (!Debugger.IsAttached)
                        {
                            _log.Error("Unhandled exception in typed packet handler:");
                            _log.Error("{0}", ex);

                            success = false;
                        }

                        if (success)
                            newPayload = serializer.Serialize(packet);
                    }
                }

                if (!newPayload.Span.SequenceEqual(payload.Span))
                    modified = true;

                payload = newPayload;
            }

            _log.Debug("{0}: {1} ({2} bytes{3}{4})", args.Direction.ToDirectionString(),
                name ?? args.Code.ToString(), payload.Length, forged ? ", forged" : string.Empty,
                modified ? ", modified" : string.Empty, silenced ? ", silenced" : string.Empty);

            args.Payload = payload;
            args.Silence = silenced;
        }
    }
}
