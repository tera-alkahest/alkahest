from Alkahest.Core.Net.Protocol.Packets import *

# The special function __start__ is invoked on startup. The proxies parameter
# is an array of Alkahest.Core.Net.GameProxy instances.
def __start__(proxies):
    for proc in map(lambda x: x.Processor, proxies):
        proc.AddHandler[CCheckVersionPacket](_handle_check_version)

    # __log__ is a special variable that holds an Alkahest.Core.Logging.Log
    # instance created specifically for this script package.
    __log__.Basic("Started example script")

# The special function __stop__ is invoked on shutdown and receives the same
# array that __start__ did.
def __stop__(proxies):
    for proc in map(lambda x: x.Processor, proxies):
        proc.RemoveHandler[CCheckVersionPacket](_handle_check_version)

    __log__.Basic("Stopped example script")

def _handle_check_version(client, direction, packet):
    for ver in packet.VersionValues:
        __log__.Info("Client reported version: {0}", ver.Value)

    return True
