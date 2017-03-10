using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Alkahest.Core
{
    public static class Assert
    {
        public static bool Enabled { get; set; }

        static readonly Exception _exception = new Exception();

        public static void Check(Expression<Func<bool>> condition)
        {
            if (!Enabled)
                return;

            var cond = condition.Compile()();
            var msg = $"Assertion failed: {condition.Body}";

            Debug.Assert(cond, msg);
        }

        public static Exception Unreachable()
        {
            if (Enabled)
                Debug.Assert(false, "Unreachable code executed.");

            return _exception;
        }
    }
}
