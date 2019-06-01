using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System.Linq;

namespace Alkahest.Commands
{
    sealed class PurgeCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(PurgeCommand));

        bool _everything;

        public PurgeCommand()
            : base("Packager", "purge", "Uninstall all packages")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [OPTIONS]",
                string.Empty,
                "Available options:",
                {
                    "e|everything",
                    $"Enable/disable removing everything in package directories (defaults to `{_everything}`)",
                    e => _everything = e != null
                },
            };
        }

        protected override int Invoke(string[] args)
        {
            var mgr = new PackageManager();

            _log.Basic("Packages to be uninstalled:");
            _log.Basic(string.Empty);

            foreach (var pkg in mgr.LocalPackages.Values)
                _log.Basic("  {0}", pkg.Name);

            _log.Basic(string.Empty);

            var count = 0;

            foreach (var pkg in mgr.LocalPackages.Values.ToArray())
            {
                _log.Info("Uninstalling package {0}...", pkg.Name);

                mgr.Uninstall(pkg, _everything);

                count++;
            }

            _log.Basic("Uninstalled {0} packages", count);

            return 0;
        }
    }
}
