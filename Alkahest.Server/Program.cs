using System;
using System.Runtime;

namespace Alkahest.Server
{
    static class Program
    {
        static int Main(string[] args)
        {
            Console.Title = Application.Name;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            return Application.Run(args);
        }
    }
}
