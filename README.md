# <img src="https://raw.githubusercontent.com/tera-alkahest/alkahest/master/Alkahest.ico" width="32"> Alkahest

[![Latest Release](https://img.shields.io/github/release/tera-alkahest/alkahest/all.svg)](https://github.com/tera-alkahest/alkahest/releases)
[![NuGet Package](https://img.shields.io/nuget/v/Alkahest.Core.svg)](https://www.nuget.org/packages/Alkahest.Core)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/tera-alkahest/alkahest?svg=true)](https://ci.appveyor.com/project/tera-alkahest/alkahest)
[![Discord Server](https://discordapp.com/api/guilds/576893607701905439/widget.png)](https://discord.io/alkahest)

**Alkahest** is a proxy server for
[TERA](https://en.wikipedia.org/wiki/TERA_%28video_game%29). At its core, it is
simply a server that relays communication between the game client and server.
Its main usefulness lies in its extensibility; you can write plugins that can
inspect packets, modify them, and send newly constructed packets. This opens up
a lot of possibilities for integrating the game with other software, as well as
adding entirely new features to the game, so long as you can do so within the
framework of the game's network protocol.

This project was started out of a need to have a program similar to
[tera-proxy](https://github.com/tera-proxy), but written in a .NET language so
that it could easily use the Windows Presentation Foundation libraries for the
purpose of making a UI overlay for the game. It has since grown to be a more
general-purpose framework for TERA modding, featuring tools and APIs for
accessing data stored with the game client.

## Features

* **.NET plugins:** Plugins can be written in any .NET language, including C#,
  F#, Nemerle, etc.
* **C# scripting:** A default plugin enables scripting with C#, via Roslyn.
* **Packet manipulation:** Packets can easily be intercepted, modified, or even
  constructed from scratch, in either raw or typed form.
* **Fast packet serialization**: Specialized serialization functions are
  automatically generated and compiled at runtime, making packet serialization
  fast and painless.
* **Extensive region support:** Almost all TERA regions are supported: DE, FR,
  JP, NA, RU, SE, TH, TW, and UK.
* **Proxy interoperability:** Alkahest supports seamless interoperability with
  [TERA Toolbox](https://github.com/tera-toolbox),
  [Shinra Meter](https://github.com/neowutran/ShinraMeter), and other similar
  projects.
* **Reusable core library:** The Alkahest server is merely a wrapper around the
  `Alkahest.Core` library which can be embedded in any .NET application.
* **Client analysis tools:** Data mining tools allow extraction of game
  messages, system messages, and data center keys, as well as decryption of the
  client's data center files.
* **Packet logging:** Compressed packet logs can be saved for later parsing and
  analysis.
* **Packet parser:** An offline packet parsing tool can generate text dumps of
  packet logs and analyze raw packet structures to find arrays and strings.

## Installation

[Archives with compiled binaries are available from the releases page.](https://github.com/tera-alkahest/alkahest/releases)

Alkahest requires .NET Framework 4.7.2 to run.

If you want to build Alkahest from source, you will need Visual Studio 2019 (any
edition). The code base is written in C# 8.0, so earlier versions will not work.

After cloning the repository, make sure to run:

```bash
git submodule update --init --recursive
```

(Or alternatively, clone with the `--recursive` option.)

Next, simply open `Alkahest.sln` and build it with the `Debug` + `Any CPU`
configuration. All build artifacts will end up in the `Build` directory.

If you want to build the NuGet package, run this command after building:

```bash
msbuild /t:Pack Alkahest.Core
```

This will create a file named something like `Alkahest.Core.1.0.0.nupkg` in the
`Build` directory.

## Configuration

After you have installed Alkahest, you will need to configure it. This is done
in the `Alkahest.exe.config` file. You can find that file in the `Build`
directory if you have built Alkahest from source, or in the directory you
installed Alkahest to.

The most important configuration values you will need to change are:

* `logLevel`: Most users should set this to `basic`. You can set it higher if
  you do not mind some extra output. Developers should probably leave this at
  `debug`.
* `loggers`: If you do not care much about keeping logs around, remove the
  `file` logger from this list to save disk space.
* `disablePlugins`: Most users can add `packet-logger` to this list to save disk
  space, as they likely will not need packet logs.
* `regions`: Set this to any of `de`, `fr`, `jp`, `na`, `ru`, `se`, `th`, `tw`,
  and `uk`, depending on which region(s) you intend to play in.

There are many other configuration values that you can play with, but you do not
need to change them if all you want is to use Alkahest for a single TERA client
on your local machine.

## Usage

Once you have configured Alkahest, run `Alkahest.exe` to start it. Once Alkahest
finishes initializing, and if everything went fine, you should be able to just
start TERA and play.

For some regions, you may need to log into TERA first, then start Alkahest just
before actually launching the game. This is necessary because some regions use
the same host name for logging in and for retrieving the server list, the
latter of which gets redirected by Alkahest.

Note that, by default, Alkahest will adjust your `hosts` file so that the host
name that the TERA launcher fetches the official server list from will be
redirected to wherever Alkahest is configured to be listening. This is necessary
so that Alkahest can give the client a modified server list where all IP
addresses point to where Alkahest is listening for each server. Alkahest will
also install root certificates if the region you are playing on requires HTTPS
for the server list. Both of these actions require administrative privileges,
so you must run Alkahest as administrator.

If Alkahest terminates abnormally, you can run `Alkahest.exe serve -c` on the
command line to clean up the aforementioned system changes.

When a new version of Alkahest is released, you can run `Alkahest.exe upgrade`
to upgrade en existing installation.

## Extensibility

Alkahest can be exteneded with
[plugins](https://github.com/tera-alkahest/alkahest/wiki/Plugin-Development) and
[script packages](https://github.com/tera-alkahest/alkahest/wiki/Script-Development).
Script packages are the preferred way to add functionality to Alkahest as they
can be distributed via the
[package registry](https://github.com/tera-alkahest/alkahest-registry). See
[this page](https://github.com/tera-alkahest/alkahest/wiki/Versioning-and-Stability)
for information about versioning and API/ABI stability.

Plugins can be installed simply by dropping the compiled assembly into the
`Plugins` directory. Alkahest will load and start/stop it automatically. A list
of all known plugins can be found
[here](https://github.com/tera-alkahest/alkahest/wiki/Known-Plugins).

Script packages can be managed with Alkahest's package management commands:

* `info`: Show detailed information for given packages.
* `install`: Install given package(s).
* `purge`: Uninstall all packages.
* `search`: Search for packages by regex.
* `uninstall`: Uninstall given package(s).
* `update`: Update given package(s) or all packages.

For example, `Alkahest.exe install example` installs the `example` package,
`Alkahest.exe update` updates all installed packages, `Alkahest.exe search foo`
finds all packages containing the string `foo` in either name or description,
etc.

## Disclaimer

Technically, using Alkahest could be considered a violation of the terms of
service for all TERA regions. Historically, most publishers have chosen to
tolerate programs such as Shinra Meter, tera-proxy, Alkahest, etc as long as
they are not used for malicious purposes. You will almost certainly be fine as
long as you do not do anything really stupid. That said, I take absolutely no
responsibility if you do manage to get yourself banned.

Also, Alkahest is meant to enable players to write useful plugins that can
enhance the TERA experience. It is not meant to enable cheating of any sort. It
may or may not be the case that some aspects of TERA's network protocol can be
exploited due to poor design (mainly trusting the client too much). Either way,
I do *not* condone using Alkahest for this, and I *certainly* will not support
such usage. I would encourage people to report such exploits to the TERA
developers (usually through whichever publisher your server is at).

## Acknowledgements

Please see [ACKNOWLEDGEMENTS.md](ACKNOWLEDGEMENTS.md).

## Contributing

Please see [CONTRIBUTING.md](.github/CONTRIBUTING.md).

## License

Please see [LICENSE.md](LICENSE.md).

## Donations

[![Liberapay Receiving](http://img.shields.io/liberapay/receives/alkahest.svg?logo=liberapay)](https://liberapay.com/alkahest/donate)
[![Liberapay Patrons](http://img.shields.io/liberapay/patrons/alkahest.svg?logo=liberapay)](https://liberapay.com/alkahest)

If you like this project and would like to support the core developers, you
might consider donating. Please only donate if you want to and have the means to
do so; we want to be very clear that the project will always be available for
free under a permissive license, and you should not feel obligated to donate or
pay for it in any way.
