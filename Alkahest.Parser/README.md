# Parser Application

This tool can parse packet log files created by the Alkahest server. It can
create hex dumps, deserialize well-known packets, and analyze packet payloads
for potential arrays and strings. It also features a roundtrip mode that can be
used to verify that packet structures in `Alkahest.Core` are correct.

## Usage

To parse a log file with default settings, simply say:

    $ alkahest-parser.exe 2017-03-23_02-46-40-249.pkt

The result will be written to `2017-03-23_02-46-40-249.txt`. You can instead
specify the output file with the `--output` option.

The `--regex` option can be used to specify one or more regexes to apply to
opcode names. If none of the regexes match, the packet will be ignored. If no
regexes are specified with this option, all packets will be processed.

You can give the `--stats` option to get some statistics about the packets that
were processed. The `--summary` option will print a list of the known and
unknown packets, along with the amount encountered and their minimum, maximum,
and average sizes.

All options have shorthand variants (e.g. `-o` for `--output`). Boolean options
can be controlled by appending a `+` or `-` to the option name. Use the `--help`
option for more information.

### Controlling Parsing

The `--hex-dump` option specifies which packets should be hex dumped. `none`
will disable hex dumps, `unknown` (default) will do hex dumps for packets with
unknown structure, and `all` will do hex dumps for all packets.

The `--parse` option can be used to control whether packets with known
structures will be deserialized (enabled by default).

The `--backend` option can be given to specify which packet serialization
backend should be used. `reflection` (default) will use the reflection-based
backend which has fast startup time but is slower for very large files.
`compiler` will use the runtime-compiling backend which has slower startup but
is significantly faster on very large files. Note that the `compiler` backend
does not include some of the sanity checks that `reflection` does.

The `--roundtrips` option specifies how many times packets should be
deserialized and reserialized during parsing. The default is zero. This option
is very effective at catching invalid packet structures.

### Controlling Analysis

Analysis can be enabled with the `--analyze` option. As with `--hex-dump`, it
accepts `none` (default), `unknown`, and `all`. When a packet is analyzed, a
list of potential arrays and strings will be printed after the hex dump.

The `--min-string-length` option can be used to specify how long a string must
be for it to be considered a potential string. You can use this to filter out
unlikely strings when you know that the strings in a packet will be of a certain
minimum length.

If you specify `--allow-white-space`, strings that consist purely of white space
characters will be considered potential strings. You can also use
`--allow-control-chars` to specify whether strings that contain control
characters (line feeds, tabs, etc) should be considered valid. Both of these are
off by default.
