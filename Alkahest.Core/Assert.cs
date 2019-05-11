using System;

namespace Alkahest.Core
{
    public static class Assert
    {
        static readonly Exception _unreachable =
            new Exception("Unreachable code executed.");

        public static void Check(bool condition, string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (!condition)
                throw new Exception($"Assertion failed: {message}");
        }

        public static Exception Unreachable()
        {
            return _unreachable;
        }
    }
}
