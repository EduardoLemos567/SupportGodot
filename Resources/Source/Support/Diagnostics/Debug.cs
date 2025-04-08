using System;
using System.Diagnostics;

namespace Support.Diagnostics;

public static class Debug
{
    public class AssertException(string? msg) : Exception(msg);
    [Conditional("DEBUG")]
    public static void ThrowAssert(object msg) => throw new AssertException(msg.ToString());
    [Conditional("DEBUG")]
    public static void Assert(bool condition, object msg)
    {
        if (!condition) { ThrowAssert(msg); }
    }
    [Conditional("DEBUG")]
    public static void Assert(bool preCondition, Func<bool> condition, object msg)
    {
        if (preCondition && !condition.Invoke()) { ThrowAssert(msg); }
    }
}
