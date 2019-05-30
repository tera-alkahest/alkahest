from Alkahest.Core.Net.Game.Packets import *

LOG = None

# The special function __start__ is invoked on startup. The proxies parameter is
# an array of Alkahest.Core.Net.GameProxy instances. The log parameter is an
# Alkahest.Core.Logging.Log instance created specifically for this script
# package.
def __start__(proxies, log):
    global LOG
    LOG = log

    for proc in map(lambda x: x.Processor, proxies):
        proc.AddHandler[CCheckVersionPacket](_handle_check_version)

    LOG.Basic("Started example script")

# The special function __stop__ is invoked on shutdown and receives the same
# parameters that __start__ did.
def __stop__(proxies, log):
    for proc in map(lambda x: x.Processor, proxies):
        proc.RemoveHandler[CCheckVersionPacket](_handle_check_version)

    LOG.Basic("Stopped example script")

def _handle_check_version(client, direction, packet):
    for ver in packet.Versions:
        LOG.Info("Client reported version: {0}", ver.Value)

    return True
