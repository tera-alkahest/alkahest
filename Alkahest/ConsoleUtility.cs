using System;
using Vanara.PInvoke;

namespace Alkahest
{
    static class ConsoleUtility
    {
        public static void AddConsoleEventHandler(Kernel32.PHANDLER_ROUTINE handler)
        {
            try
            {
                Kernel32.SetConsoleCtrlHandler(handler, true);
            }
            catch (Exception ex) when (IsDllException(ex))
            {
            }
        }

        public static void RemoveConsoleEventHandler(Kernel32.PHANDLER_ROUTINE handler)
        {
            try
            {
                Kernel32.SetConsoleCtrlHandler(handler, false);
            }
            catch (Exception ex) when (IsDllException(ex))
            {
            }
        }

        static bool IsDllException(Exception exception)
        {
            return exception is DllNotFoundException ||
                exception is EntryPointNotFoundException;
        }
    }
}
