# Python Plugin

This plugin lets you write scripts for Alkahest in Python instead of having to
compile a .NET assembly. It's based on IronPython and has mostly complete
support for Python 2.7.

With IronPython, scripts are compiled to native code when loaded, so while not
as fast as C# code, they'll still be much faster than many other scripting
languages.

## Configuration

Configuration is done in the `alkahest-python.dll.config` file in the `Plugins`
directory.

You shouldn't need to change anything by default, but you can change the
`disablePackages` list if you need to temporarily disable a script package.

## Usage

The unit of encapsulation is a script package. Each directory under
`Python` is considered a package and will be loaded separately. Within a
package, you can have as many Python files as you need. To use someone else's
script package, simply drop it into its own directory.

A package must at least have a file called `__init__.py`. This file must have
the special `__start__` and `__stop__` functions. These are invoked on startup
and shutdown, respectively, and receive an array of
`Alkahest.Core.Net.GameProxy` objects. They're the Python equivalents of the
`Start` and `Stop` methods on the `Alkahest.Core.Plugins.IPlugin` interface.

Within Python scripts, the special name `__log__` refers to an
`Alkahest.Core.Logging.Log` object created specifically for the current
script package. All logging should go through this object; normal console I/O
(such as `print`) has no effect.

A reference to the `Alkahest.Core` assembly is added automatically, so you can
readily `import` anything from it.

Note that scripts are evaluated before the `__start__` function is invoked.
This lets you do any initialization you need at the module level.

See the [example](Python/example) package to get an idea of how a Python script
should look.
