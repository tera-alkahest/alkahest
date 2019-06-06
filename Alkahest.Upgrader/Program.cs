using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace Alkahest.Upgrader
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                try
                {
                    using var proc = Process.GetProcessById(int.Parse(args[0]));

                    proc.WaitForExit();
                }
                catch (ArgumentException)
                {
                    // The parent process disappeared already.
                }

                using (var zip = ZipFile.OpenRead(args[1]))
                {
                    var dest = args[2];

                    foreach (var entry in zip.Entries)
                    {
                        var dir = Path.GetDirectoryName(entry.FullName);

                        if (dir != string.Empty)
                            Directory.CreateDirectory(Path.Combine(dest, dir));

                        entry.ExtractToFile(Path.Combine(dest, entry.FullName), true);
                    }
                }

                File.Delete(args[1]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Error.WriteLine();
                Console.Error.WriteLine("Could not complete the upgrade process:");
                Console.Error.WriteLine(ex);
                Console.Error.WriteLine("You may need to download the latest release manually");
                Console.Error.WriteLine("Press any key to exit");

                Console.ReadLine();
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
