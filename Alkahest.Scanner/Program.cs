using System;
using System.Runtime;

namespace Alkahest.Scanner
{
    static class Program
    {
        static int Main(string[] args)
        {
            Console.Title = Application.Name;
            GCSettings.LatencyMode = GCLatencyMode.Batch;

            return Application.Run(args);
        }
    }
}
