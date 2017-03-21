using System;

namespace Alkahest.Core
{
    public static class Assert
    {
        static readonly Exception _exception = new Exception(
            "Unreachable code executed.");

        public static void Check(bool condition, string message)
        {
            if (!condition)
                throw new Exception($"Assertion failed: {message}");
        }

        public static Exception Unreachable()
        {
            return _exception;
        }
    }
}
