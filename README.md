# ![Alkahest](Alkahest.ico) Alkahest

[![Latest Release](https://img.shields.io/github/release/alexrp/alkahest/all.svg)](https://github.com/alexrp/alkahest/releases)
[![NuGet Package](https://img.shields.io/nuget/v/Alkahest.Core.svg)](https://www.nuget.org/packages/Alkahest.Core)
[![Build Status](https://ci.appveyor.com/api/projects/status/github/alexrp/alkahest?svg=true)](https://ci.appveyor.com/project/alexrp/alkahest)

**Alkahest** is a proxy server for
[TERA](https://en.wikipedia.org/wiki/TERA_%28video_game%29). At its core, it's
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
* **Client analysis tools:** Data mining tools allow extraction of opcodes,
  system messages, and data center keys, as well as decryption of the client's
  data center files.
* **Complete region support:** All TERA regions are supported: EU, JP, KR, NA,
  RU, TH, and TW.
* **Fast packet serialization**: Specialized serialization functions are
  automatically generated and compiled at runtime, making packet serialization
  fast and painless.
* **Packet editing:** Packets can easily be intercepted, modified, or even
  constructed from scratch, in either raw or typed form.
* **Packet logging:** Compressed packet logs can be saved for later parsing and
  analysis.
* **Packet parser:** An offline packet parsing tool can generate text dumps of
  packet logs and analyze raw packet structures to find arrays and strings.
* **Python scripting:** One of the default plugins implements Python scripting
  support using IronPython.
* **Reusable core library:** The Alkahest server is only a wrapper around the
  `Alkahest.Core` library which can be embedded in any .NET application.

## Installation

[Installers and archives are available from the releases page.](https://github.com/alexrp/alkahest/releases)

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
nuget pack -Symbols Alkahest.Core
```

This will create a file named something like `Alkahest.Core.1.0.0-alpha4`.

## Configuration

After you've built Alkahest, you will need to configure it. This is done in the
`alkahest-server.exe.config` file. You can find that file in the `Build`
directory if you've built Alkahest from source, or in the directory you
installed Alkahest to.

The most important configuration values you'll need to change are:

* `logLevel`: Most users should set this to `basic`. You can set it higher if
  you don't mind some extra output. Developers should probably leave this at
  `debug`.
* `loggers`: If you don't care much about keeping logs around, remove the `file`
  logger from this list to save disk space.
* `disablePlugins`: Remove any core plugins from this list that you want to use.
  You can also add plugins here that you want to temporarily disable, such as
  the `packet-logger` plugin which is only useful to developers.
* `region`: Set this to `eu`, `jp`, `kr`, `na`, `ru`, `th`, or `tw`, or if
  you're playing in EU, `de`, `fr`, or `uk`.

There are many other configuration values that you can play with, but you don't
need to change them if all you want is to use Alkahest for a single TERA client
on your local machine.

If you want to run Alkahest with tera-proxy, see
[this wiki page](https://github.com/alexrp/alkahest/wiki/Interoperability).

## Usage

Once you've configured Alkahest, run `alkahest-server.exe` to start it. Once
Alkahest finishes initializing, and if everything went fine, you should be able
to just start TERA and play.

For some regions, you may need to log into TERA first, then start Alkahest just
before actually launching the game. This is necessary because some regions use
the same host name for logging in and for retrieving the server list, the
latter of which gets redirected by Alkahest. For KR specifically, you need to
start Alkahest right after launching the game, during the splash screen.

Note that, by default, Alkahest will adjust your `hosts` file so that the host
name that the TERA launcher fetches the official server list from will be
redirected to wherever Alkahest is configured to be listening. This is necessary
so that Alkahest can give the client a modified server list where all IP
addresses point to where Alkahest is listening for each server. Modifying the
`hosts` file requires administrative privileges, so you must run Alkahest as
administrator.

## Plugins

The plugin system in Alkahest is what adds actual functionality to the proxy
server. Plugins are installed by dropping them into the `Plugins` directory.

A list of known plugins is maintained on
[this wiki page](https://github.com/alexrp/alkahest/wiki/Known-Plugins). If
you're interested in plugin development, see
[this wiki page](https://github.com/alexrp/alkahest/wiki/Plugin-Development).

## Disclaimer

Technically, using Alkahest could be considered a violation of the terms of
service for all TERA regions. Historically, most publishers have chosen to
tolerate programs such as Shinra Meter, tera-proxy, Alkahest, etc as long as
they're not used for malicious purposes. You'll almost certainly be fine as long
as you don't do anything really stupid. That said, I take absolutely no
responsibility if you do manage to get yourself banned.

Also, Alkahest is meant to enable players to write useful plugins that can
enhance the TERA experience. It is not meant to enable cheating of any sort. It
may or may not be the case that some aspects of TERA's network protocol can be
exploited due to poor design (mainly trusting the client too much). Either way,
I do *not* condone using Alkahest for this, and I *certainly* won't support such
usage. I'd encourage people to report such exploits to the TERA developers
(usually through whichever publisher your server is at).

## Acknowledgements

Please see [ACKNOWLEDGEMENTS.md](ACKNOWLEDGEMENTS.md).

## Contributing

Please see [.github/CONTRIBUTING.md](CONTRIBUTING.md).

## License

Please see [LICENSE.md](LICENSE.md).
