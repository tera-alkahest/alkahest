using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Plugins;
using IronPython.Compiler;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Alkahest.Plugins.Python
{
    public sealed class PythonPlugin : IPlugin
    {
        const string PackageFile = "__init__.py";

        public string Name { get; } = "python";

        static readonly Log _log = new Log(typeof(PythonPlugin));

        readonly Dictionary<string, CompiledCode> _scripts =
            new Dictionary<string, CompiledCode>();

        public void Start(GameProxy[] proxies)
        {
            var pkg = Configuration.PackageDirectory;

            Directory.CreateDirectory(pkg);

            foreach (var dir in Directory.EnumerateDirectories(pkg))
            {
                var name = Path.GetFileName(dir);

                if (Configuration.DisablePackages.Contains(name))
                    continue;

                var engine = IronPython.Hosting.Python.CreateEngine();
                var opts = (PythonCompilerOptions)engine.GetCompilerOptions();

                opts.AbsoluteImports = true;
                opts.AllowWithStatement = true;
                opts.Optimized = true;
                opts.PrintFunction = true;
                opts.TrueDivision = true;
                opts.UnicodeLiterals = true;

                var paths = engine.GetSearchPaths();

                paths.Add(Configuration.StdLibDirectory);

                engine.SetSearchPaths(paths);

                var io = engine.Runtime.IO;

                io.SetOutput(Stream.Null, io.OutputEncoding);
                io.SetErrorOutput(Stream.Null, io.ErrorEncoding);
                io.SetInput(Stream.Null, io.InputEncoding);

                ((dynamic)IronPython.Hosting.Python.GetClrModule(engine))
                    .AddReference(typeof(Assert).Assembly.FullName);

                ((dynamic)IronPython.Hosting.Python.GetBuiltinModule(engine)).__log__ =
                    new Log(typeof(PythonPlugin), name);

                var src = engine.CreateScriptSourceFromFile(
                    Path.Combine(dir, PackageFile));

                CompiledCode script;

                try
                {
                    script = src.Compile();
                    script.Execute();
                }
                catch (Exception e)
                {
                    if (e is SyntaxErrorException syn)
                    {
                        _log.Error("Syntax error in package {0}:", name);
                        _log.Error("{0} ({1}, {2}): {3}", syn.SourcePath,
                            syn.Line, syn.Column, syn.Message);
                    }
                    else
                    {
                        _log.Error("Failed to initialize package {0}:", name);
                        _log.Error(e.ToString());
                    }

                    continue;
                }

                _scripts.Add(name, script);
            }

            var count = 0;

            foreach (var kvp in _scripts)
            {
                try
                {
                    ((dynamic)kvp.Value.DefaultScope).__start__(proxies.ToArray());
                }
                catch (Exception e)
                {
                    _log.Error("Failed to start package {0}:", kvp.Key);
                    _log.Error(e.ToString());

                    continue;
                }

                _log.Info("Started package {0}", kvp.Key);

                count++;
            }

            _log.Basic("Started {0} packages", count);
        }

        public void Stop(GameProxy[] proxies)
        {
            var count = 0;

            foreach (var kvp in _scripts)
            {
                try
                {
                    ((dynamic)kvp.Value.DefaultScope).__stop__(proxies.ToArray());
                }
                catch (Exception e)
                {
                    _log.Error("Failed to stop package {0}:", kvp.Key);
                    _log.Error(e.ToString());

                    continue;
                }

                _log.Info("Stopped package {0}", kvp.Key);

                count++;
            }

            _log.Basic("Stopped {0} packages", count);
        }
    }
}
