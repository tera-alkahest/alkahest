# Alkahest C# Plugin

This plugin lets you write scripts for Alkahest in C#. It is based on Roslyn and
supports C# 8.0.

Scripts are compiled to native code when loaded, so they are no slower than if
you had manually compiled C# code to a .NET assembly. Additionally, scripts are
compiled with debugging information, so if you attach a debugger to Alkahest,
you will be able to debug your scripts. In other words, you get all the benefits
of writing a plugin in C# but without the hassle of having to compile and
distribute the plugin assembly.

## Configuration

Configuration is done in the `alkahest-csharp.dll.config` file in the `Plugins`
directory.

You should not need to change anything by default, but you can change the
`disablePackages` list if you need to temporarily disable a script package.

## Usage

The unit of encapsulation is a script package. Each directory under `Packages`
is considered a script package, and can contain as many C# files (with the `.cs`
extension) as needed. All script packages are compiled together into the same
.NET assembly at startup, so they can use code from each other.

A package must have at least a file called `Main.cs`. This file must contain a
class exposing the special `__Start__` and `__Stop__` methods. These are invoked
on startup and shutdown, respectively, and receive an
`Alkahest.Plugins.CSharp.CSharpScriptContext` object. This object provides
functionality such as packet handling, data center access, logging, etc.

A reference to the `Alkahest.Core` assembly is added automatically, so you can
readily access anything from it, e.g. via `using` directives. References to all
common .NET Framework assemblies are added as well.

See the [wiki page](https://github.com/tera-alkahest/alkahest/wiki/Script-Development)
for more information on script development.
