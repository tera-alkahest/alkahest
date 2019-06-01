using Alkahest.Commands;
using Alkahest.Core.Logging;
using Mono.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Alkahest
{
    static class Program
    {
        static readonly Log _log = new Log(typeof(Program));

        public static string Name { get; } = Assembly.GetExecutingAssembly().GetName().Name;

        static int Main(string[] args)
        {
            static string Default(string str)
            {
                // Pretty awful hack to implement a default command.
                if (str == $"Use `{Name} help` for usage.")
                    throw new DefaultCommandException();

                return str;
            }

            var set = new CommandSet(Name, Default)
            {
                $"Usage: {Name} COMMAND [OPTIONS]",
                $"Use `{Name} help COMMAND` for help on a specific command.",
                string.Empty,
                "Available commands:",
                string.Empty,
                new HelpCommand(),
                new DecryptCommand(),
                new DumpCommand(),
                new InfoCommand(),
                new InstallCommand(),
                new ParseCommand(),
                new PurgeCommand(),
                new ScanCommand(),
                new SearchCommand(),
                new ServeCommand(),
                new UninstallCommand(),
                new UpdateCommand(),
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "v|version",
                    "Print version and exit",
                    v =>
                    {
                        Console.WriteLine("{0} {1}", Name, Assembly.GetExecutingAssembly()
                            .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
                        Environment.Exit(0);
                    }
                }
            };

            try
            {
                try
                {
                    return set.Run(args);
                }
                catch (DefaultCommandException)
                {
                    return set.Run(new[] { "serve" }.Concat(args));
                }
            }
            catch (OptionException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
            catch (Exception ex) when (!Debugger.IsAttached)
            {
                _log.Error("{0}", ex);
                return 1;
            }
        }
    }
}
