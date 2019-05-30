using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Plugins;
using IronPython.Compiler;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Alkahest.Plugins.Python
{
    public sealed class PythonPlugin : IPlugin
    {
        const string PackageFile = "__init__.py";

        public string Name { get; } = "python";

        static readonly Log _log = new Log(typeof(PythonPlugin));

        readonly List<(string, CompiledCode, Log)> _scripts = new List<(string, CompiledCode, Log)>();

        public void Start(GameProxy[] proxies)
        {
            var pkg = Configuration.PackageDirectory;

            Directory.CreateDirectory(pkg);

            var engine = IronPython.Hosting.Python.CreateEngine();
            var opts = (PythonCompilerOptions)engine.GetCompilerOptions();

            opts.AbsoluteImports = true;
            opts.AllowWithStatement = true;
            opts.PrintFunction = true;
            opts.TrueDivision = true;
            opts.UnicodeLiterals = true;

            var paths = engine.GetSearchPaths();

            paths.Add(Configuration.StdLibDirectory);
            paths.Add(pkg);

            engine.SetSearchPaths(paths);

            var io = engine.Runtime.IO;

            io.SetOutput(Stream.Null, io.OutputEncoding);
            io.SetErrorOutput(Stream.Null, io.ErrorEncoding);
            io.SetInput(Stream.Null, io.InputEncoding);

            var clr = (dynamic)IronPython.Hosting.Python.GetClrModule(engine);

            clr.AddReference(typeof(Assert).Assembly.FullName);
            clr.AddReference(typeof(Vector3).Assembly.FullName);
            clr.AddReference(typeof(Unsafe).Assembly.FullName);

            foreach (var dir in Directory.EnumerateDirectories(pkg))
            {
                var name = Path.GetFileName(dir);

                if (Configuration.DisablePackages.Contains(name))
                    continue;

                var src = engine.CreateScriptSourceFromFile(Path.Combine(dir, PackageFile), Encoding.UTF8);

                CompiledCode script;

                try
                {
                    script = src.Compile();
                    script.Execute();
                }
                catch (SyntaxErrorException syn)
                {
                    _log.Error("Syntax error in package {0}:", name);
                    _log.Error("{0}({1},{2}): {3}", syn.SourcePath, syn.Line, syn.Column, syn.Message);

                    continue;
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    _log.Error("Failed to initialize package {0}:", name);
                    _log.Error("{0}", e);

                    continue;
                }

                _scripts.Add((name, script, new Log(typeof(PythonPlugin), name)));
            }

            var count = 0;

            foreach (var (name, code, log) in _scripts)
            {
                try
                {
                    ((dynamic)code.DefaultScope).__start__(proxies.ToArray(), log);
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    _log.Error("Failed to start package {0}:", name);
                    _log.Error("{0}", e);

                    continue;
                }

                _log.Info("Started package {0}", name);

                count++;
            }

            _log.Basic("Started {0} packages", count);
        }

        public void Stop(GameProxy[] proxies)
        {
            var count = 0;

            foreach (var (name, code, log) in _scripts)
            {
                try
                {
                    ((dynamic)code.DefaultScope).__stop__(proxies.ToArray(), log);
                }
                catch (Exception e) when (!Debugger.IsAttached)
                {
                    _log.Error("Failed to stop package {0}:", name);
                    _log.Error("{0}", e);

                    continue;
                }

                _log.Info("Stopped package {0}", name);

                count++;
            }

            _log.Basic("Stopped {0} packages", count);
        }
    }
}
