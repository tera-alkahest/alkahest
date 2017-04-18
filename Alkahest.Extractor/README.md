# Extractor Application

This tool can extract useful data from TERA client files.

## Usage

Basic usage is of the form:

    $ alkahest-extractor.exe <command> <arguments>

Currently, these commands are supported:

* `decrypt <data center file> <key file> <iv file>`: Decrypt and decompress a
  data center file. Allows it to be read by other commands.
* `dump-json <data center file>`: Dump data center contents to a specified
  directory as JSON. The default output directory is `Json`.
* `dump-xml <data center file>`: Dump data center contents to a specified
  directory as XML. The default output directory is `Xml`.

You can specify the output file or directory for a command by giving the
`--output` option.

All options have shorthand variants (e.g. `-o` for `--output`). Use the
`--help` option for more information.
