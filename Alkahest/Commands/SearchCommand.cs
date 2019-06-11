using Alkahest.Core.Collections;
using Alkahest.Core.Logging;
using Alkahest.Packager;
using Mono.Options;
using System.Linq;
using System.Text.RegularExpressions;

namespace Alkahest.Commands
{
    sealed class SearchCommand : AlkahestCommand
    {
        const RegexOptions SearchRegexOptions =
            RegexOptions.Compiled |
            RegexOptions.Singleline;

        static readonly Log _log = new Log(typeof(SearchCommand));

        public SearchCommand()
            : base("Packager", "search", "Search for packages by regex")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} REGEX...",
            };
        }

        protected override int Invoke(string[] args)
        {
            var regexes = args.Select(x => new Regex(x, SearchRegexOptions))
                .DefaultIfEmpty(new Regex(".*", SearchRegexOptions));
            var mgr = new PackageManager();
            var match = false;

            foreach (var latest in mgr.Registry.Values.Where(pkg => regexes.All(
                x => x.IsMatch(pkg.Name) || x.IsMatch(pkg.Description))))
            {
                if (!match)
                {
                    _log.Basic(string.Empty);
                    match = true;
                }

                _log.Basic("  {0} | {1} | https://github.com/{2}/{3}", latest.Name, latest.License,
                    latest.Owner, latest.Repository);

                var desc = latest.Description.Split('\n').Select(x => x.Trim()).ToArray();

                foreach (var (i, line) in desc.WithIndex())
                {
                    _log.Basic("    {0}", line);

                    if (i != desc.Length - 1)
                        _log.Basic(string.Empty);
                }
            }

            if (!match)
                _log.Warning("No matching packages found");

            return 0;
        }
    }
}
