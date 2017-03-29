using System;
using System.Runtime.InteropServices;

namespace Alkahest.Server
{
    static class ConsoleUtility
    {
        public const int ControlCEvent = 0;

        public const int ControlBreakEvent = 1;

        public const int CloseEvent = 2;

        public const int LogOffEvent = 5;

        public const int ShutdownEvent = 6;

        public delegate bool ConsoleEventHandler(int @event);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCtrlHandler(ConsoleEventHandler handler,
            bool add);

        public static void AddConsoleEventHandler(ConsoleEventHandler handler)
        {
            try
            {
                SetConsoleCtrlHandler(handler, true);
            }
            catch (DllNotFoundException)
            {
            }
        }

        public static void RemoveConsoleEventHandler(ConsoleEventHandler handler)
        {
            try
            {
                SetConsoleCtrlHandler(handler, false);
            }
            catch (DllNotFoundException)
            {
            }
        }
    }
}
