# Scanner Application

This application scans TERA memory for data that's needed to run Alkahest. This
is currently network opcodes, system messages, and the private key and
initialization vector needed to decrypt data center files.

Whenever a new patch is deployed in any region, this data must be gathered anew
and added to the `Alkahest.Core` project if it has changed from the last
patch.

## Usage

First, start TERA and let it get to character selection. Then, run the scanner:

```bash
alkahest-scanner.exe
```

The scanner will find the TERA process, inject itself into it, and then scan for
all the relevant data. The data will be stored in text files in the output
directory, which is `Scan` by default.

Note that the scanner needs administrative privileges to work.

You can specify a different output directory by giving the `--output` option.

All options have shorthand variants (e.g. `-o` for `--output`). Use the `--help`
option for more information.
