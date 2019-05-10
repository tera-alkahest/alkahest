# Packet Logger Plugin

This plugin writes a (possibly compressed) packet log for an Alkahest instance.
This log can later be parsed and analyzed with the `alkahest-parser` tool.

## Configuration

Configuration is done in the `alkahest-packet-logger.dll.config` file in the
`Plugins` directory.

You shouldn't need to change anything by default.

## Usage

Simply start Alkahest with this plugin installed. Packet logs will be saved
automatically. Refer to the parser application's
[README.md](../Alkahest.Parser/README.md) to learn how to parse the log files.
