# C# Plugin

This plugin lets you write scripts for Alkahest in C#. It's based on Roslyn and
supports C# 8.0.

Scripts are compiled to native code when loaded, so they are no slower than if
you had manually compiled C# code to a .NET assembly. Additionally, scripts are
compiled with debugging information, so if you attach a debugger to Alkahest,
you will be able to debug your scripts. In other words, you get all the benefits
of writing a plugin C# but without the hassle of having to compile and
distribute the plugin assembly.

## Configuration

Configuration is done in the `alkahest-csharp.dll.config` file in the `Plugins`
directory.

You shouldn't need to change anything by default, but you can change the
`disablePackages` list if you need to temporarily disable a script package.

## Usage

The unit of encapsulation is a script package. Each directory under `CSharp` is
considered a package, and can contain as many C# files (with the `.csx`
extension) as needed. All script packages are compiled together into the same
.NET assembly at startup, so they can use code from each other.

<!--
A package must at least have a file called `__init__.py`. This file must have
the special `__start__` and `__stop__` functions. These are invoked on startup
and shutdown, respectively, and receive an array of
`Alkahest.Core.Net.GameProxy` objects as well as an `Alkahest.Core.Logging.Log`
instance created specifically for the current package. These functions are the
Python equivalents of the `Start` and `Stop` methods on the
`Alkahest.Core.Plugins.IPlugin` interface. All logging should go through the log
object passed to these functions; normal console I/O (such as `print`) has no
effect.
-->

A reference to the `Alkahest.Core` assembly is added automatically, so you can
readily access anything from it, e.g. via `using` directives.

See the
[csharp-example](https://github.com/tera-alkahest/alkahest-csharp-example)
package to get an idea of how a C# script package should look.
