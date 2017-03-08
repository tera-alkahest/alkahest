using System;
using System.Runtime;

namespace Alkahest.Server
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Title = Application.Name;
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Application.Run(args);
        }
    }
}
