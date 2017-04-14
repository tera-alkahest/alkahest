# ![Alkahest](Alkahest.ico) Alkahest

[![NuGet](https://img.shields.io/nuget/v/Alkahest.Core.svg)](https://www.nuget.org/packages/Alkahest.Core)
[![Build Status](https://ci.appveyor.com/api/projects/status/g4975y955sgosiej?svg=true)](https://ci.appveyor.com/project/alexrp/alkahest)
[![Discord](https://discordapp.com/api/guilds/302077598593318912/widget.png)](https://discord.gg/5GTr5mw)

Alkahest is a proxy server for [TERA](http://tera.enmasse.com). At its core,
it's simply a server that relays communication between the game client and
server. Its main usefulness lies in its extensibility; you can write plugins
that can inspect packets, modify them, and send newly constructed packets. This
opens up a lot of possibilities for integrating the game with other software,
as well as adding entirely new features to the game, so long as you can do so
within the framework of the game's network protocol.

This project was started out of a need to have a program similar to
[tera-proxy](https://github.com/meishuu/tera-proxy), but written in a .NET
language so that it could easily use the Windows Presentation Foundation
libraries for the purpose of making a UI overlay for the game. It has since
grown to be a more general purpose framework for TERA modding.

## Features

* **Complete region support:** All TERA regions are supported: EU, JP, KR, NA,
  RU, and TW.
* **Packet editing:** Packets can easily be intercepted, modified, or even
  constructed from scratch, in either raw or typed form.
* **Fast packet serialization**: Specialized serialization functions are
  automatically generated and compiled at runtime, making packet serialization
  fast and painless.
* **Packet logging:** Compressed packet logs can be saved for later parsing and
  analysis.
* **.NET plugins:** Plugins can be written in any .NET language, including C#,
  F#, Nemerle, etc.
* **Python scripting:** One of the default plugins implements Python scripting
  support using IronPython.
* **Reusable core library:** The Alkahest server is only a wrapper around the
  `Alkahest.Core` library which can be embedded in any .NET application.
* **Packet parser:** An offline packet parsing tool can generate text dumps of
  packet logs and analyze raw packet structures to find arrays and strings.

## Installation

Binary releases of Alkahest are not yet available as the project is still in
heavy development. If you want to build it, you will need Visual Studio 2017
(any edition) and .NET Framework 4.6.1. Simply open `Alkahest.sln` and build
it with the `Debug` + `Any CPU` configuration. All build artifacts will end up
in the `Build` directory. Run `alkahest-server.exe` to start the proxy server
(but see below for configuration).

## Configuration

After you've built Alkahest, you will need to configure it. This is done in the
`alkahest-server.exe.config` file in the `Build` directory.

The most important configuration values you'll need to change are:

* `logLevel`: Most users should set this to `basic`. You can set it higher if
  you don't mind some extra output. Developers should probably leave this at
  `debug`.
* `loggers`: If you don't care much about keeping logs around, remove the
  `file` logger from this list to save disk space.
* `enablePacketLogs`: Unless you're a developer, you should set this to
  `false`. Packet logs can get quite big and aren't useful to most users.
* `disablePlugins`: Remove any plugins from this list that you want to use. You
  can also add plugins here that you don't want to use.
* `region`: Set this to `na` or `eu` depending on which region you're playing
  in.

There are many other configuration values that you can play with, but you don't
need to change them if all you want is to use Alkahest for a single TERA client
on your local machine.

You should also check configuration files in the `Plugins` directory. Some
plugins may require extra configuration.

Note that rebuilding any project in the solution may overwrite configuration
files in the `Build` directory. Make backups if needed.

## Usage

Once you've configured Alkahest, run `alkahest-server.exe` to start it. Once
Alkahest finishes initializing, and if everything went fine, you should be able
to just start TERA and play.

For some regions, you may need to log into TERA first, then start Alkahest just
before actually launching the game. This is necessary because some regions use
the same domain name for logging in and for retrieving the server list, the
latter of which gets redirected by Alkahest. For KR specifically, you need to
start Alkahest right after launching the game, during the splash screen.

Note that, by default, Alkahest will adjust your `hosts` file so that the
hostname that the TERA launcher fetches the official server list from will be
redirected to wherever Alkahest is configured to be listening. This is
necessary so that Alkahest can give the client a modified server list where all
IP addresses point to where Alkahest is listening for each server. Modifying
the `hosts` file requires administrative privileges, so you must run Alkahest
as administrator.

## Disclaimer

Technically, using Alkahest could be considered a violation of the terms of
service for all TERA regions. Historically, most publishers have chosen to
tolerate programs such as Shinra Meter, tera-proxy, Alkahest, etc as long as
they're not used for malicious purposes. You'll almost certainly be fine as
long as you don't do anything really stupid. That said, I take absolutely no
responsibility if you do manage to get yourself banned.

Also, Alkahest is meant to enable players to write useful plugins that can
enhance the TERA experience. It is not meant to enable cheating of any sort. It
may or may not be the case that some aspects of TERA's network protocol can be
exploited due to poor design (mainly trusting the client too much). Either way,
I do *not* condone using Alkahest for this, and I *certainly* won't support
such usage. I'd encourage people to report such exploits to the TERA developers
(usually through whichever publisher your server is at).

## Acknowledgements

Please see [ACKNOWLEDGEMENTS.md](ACKNOWLEDGEMENTS.md).

## Contributing

Please see [CONTRIBUTING.md](CONTRIBUTING.md).

## License

Please see [LICENSE.md](LICENSE.md).
