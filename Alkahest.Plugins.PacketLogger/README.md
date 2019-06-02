# Packet Logger Plugin

This plugin writes a (possibly compressed) packet log for an Alkahest instance.
This log can later be parsed and analyzed with the `parse` command.

The packet log file format is documented
[here](https://github.com/tera-alkahest/alkahest/wiki/Packet-Log-Format).

## Configuration

Configuration is done in the `alkahest-packet-logger.dll.config` file in the
`Plugins` directory.

You should not need to change anything by default.

## Usage

Simply start Alkahest with this plugin installed. Packet logs will be saved
automatically.
