# Python Plugin

This plugin lets you write scripts for Alkahest in Python. It is based on
IronPython and has mostly complete support for Python 2.7, plus a few Python 3
features.

With IronPython, scripts are compiled to native code when loaded, so while not
as fast as C# code, they will still be much faster than many other scripting
languages.

## Configuration

Configuration is done in the `alkahest-python.dll.config` file in the `Plugins`
directory.

You should not need to change anything by default, but you can change the
`disablePackages` list if you need to temporarily disable a script package.

## Usage

The unit of encapsulation is a script package. Each directory under `Python` is
considered a package, and can contain as many Python files (with the `.py`
extension) as needed. Packages are loaded individually by the same IronPython
execution engine, so they can `import` from each other.

A package must have at least a file called `__init__`. This file must contain a
class exposing the special `__Start__` and `__Stop__` methods. These are invoked
on startup and shutdown, respectively, and receive an array of
`Alkahest.Core.Net.Game.GameProxy` objects as well as an
`Alkahest.Core.Logging.Log` object created specifically for the current package.
All logging should go through the log object passed to these functions, rather
than normal console I/O.

A package must at least have a file called `__init__.py`. This file must have
the special `__start__` and `__stop__` functions. These are invoked on startup
and shutdown, respectively, and receive an array of
`Alkahest.Core.Net.Game.GameProxy` objects as well as an
`Alkahest.Core.Logging.Log` object created specifically for the current package.
All logging should go through the log object passed to these functions, rather
than normal console I/O.

A reference to the `Alkahest.Core` assembly is added automatically, so you can
readily `import` anything from it. References to all common .NET Framework
assemblies are added as well.

Note that scripts are evaluated before the `__start__` function is invoked. This
lets you do any initialization you need at the module level.

See the
[python_example](https://github.com/tera-alkahest/alkahest-python-example)
package to get an idea of how a Python script package should look.
