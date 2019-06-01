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

                foreach (var line in latest.Description.Split('\n').Select(x => x.Trim()))
                {
                    _log.Basic("    {0}", line);
                    _log.Basic(string.Empty);
                }
            }

            if (!match)
                _log.Warning("No matching packages found");

            return 0;
        }
    }
}
