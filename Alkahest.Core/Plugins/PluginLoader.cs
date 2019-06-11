using Alkahest.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alkahest.Core.Plugins
{
    public sealed class PluginLoader
    {
        const BindingFlags ConstructorFlags =
            BindingFlags.DeclaredOnly |
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        const string ClassSuffix = "Plugin";

        const string NamespacePrefix = "Alkahest.Plugins.";

        static readonly Log _log = new Log(typeof(PluginLoader));

        public PluginContext Context { get; }

        public IReadOnlyCollection<IPlugin> Plugins { get; }

        object _token;

        public PluginLoader(PluginContext context, string directory, string pattern, string[] exclude)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));

            Directory.CreateDirectory(directory);

            if (exclude == null)
                exclude = Array.Empty<string>();

            Plugins = (from file in Directory.EnumerateFiles(directory, pattern)
                       from type in Assembly.UnsafeLoadFrom(file).DefinedTypes
                       where type.ImplementedInterfaces.Contains(typeof(IPlugin))
                       let ctor = type.GetConstructor(ConstructorFlags, null, new[] { typeof(PluginContext) }, null)
                       let plugin = (IPlugin)ctor.Invoke(new[] { context })
                       where !exclude.Contains(plugin.Name)
                       select EnforceConventions(plugin)).ToArray();
        }

        static IPlugin EnforceConventions(IPlugin plugin)
        {
            var name = plugin.Name;

            if (name.Any(c => char.IsUpper(c)))
                throw new PluginException($"{name}: Plugin name must not contain upper case characters.");

            var type = plugin.GetType();

            if (!type.Name.EndsWith(ClassSuffix))
                throw new PluginException($"{name}: Plugin class name must end with '{ClassSuffix}'.");

            if (!type.Namespace.StartsWith(NamespacePrefix))
                throw new PluginException($"{name}: Plugin namespace must start with '{NamespacePrefix}'.");

            var asm = type.Assembly;
            var asmName = $"alkahest-{name}";

            if (asm.GetName().Name != asmName)
                throw new PluginException($"{name}: Plugin assembly name must be '{asmName}'.");

            var fileName = asmName + ".dll";

            if (Path.GetFileName(asm.Location.ToLowerInvariant()) != fileName)
                throw new PluginException($"{name}: Plugin file name must be '{fileName}'.");

            return plugin;
        }

        public void Start()
        {
            _token = Context.Data.Freeze();

            foreach (var p in Plugins)
            {
                p.Start();

                _log.Info("Started plugin {0}", p.Name);
            }

            _log.Basic("Started {0} plugins", Plugins.Count);
        }

        public void Stop()
        {
            foreach (var p in Plugins)
            {
                p.Stop();

                _log.Info("Stopped plugin {0}", p.Name);
            }

            _log.Basic("Stopped {0} plugins", Plugins.Count);

            Context.Data.Thaw(_token);
        }
    }
}
