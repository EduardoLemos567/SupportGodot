using System;
using System.Diagnostics;
using Godot;

namespace Support.Diagnostics;

public static class Debug
{
    private static readonly Stopwatch timing = Stopwatch.StartNew();
    public enum PRINT_COLOR
    {
        BLACK, RED, GREEN, YELLOW, BLUE, MAGENTA, PINK, PURPLE, CYAN, WHITE, ORANGE, GRAY
    }
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
    [Conditional("DEBUG")]
    public static void Warn(bool condition, object msg)
    {
        if (condition) { PrintWarn(msg); }
    }
    [Conditional("DEBUG")]
    public static void Warn(bool preCondition, Func<bool> condition, object msg)
    {
        if (preCondition && condition.Invoke()) { PrintWarn(msg); }
    }
    [Conditional("DEBUG")]
    public static void Print(params object[] objs) => GD.Print($"[{Timing()}] {string.Join(',', objs)}");
    [Conditional("DEBUG")]
    public static void PrintColor(string msg, PRINT_COLOR color) => GD.PrintRich($"[color={color.ToString().ToLower()}][{Timing()}] {msg}[/color]");
    [Conditional("DEBUG")]
    public static void PrintWarn(params object[] objs) => GD.PushWarning($"[{Timing()}] {string.Join(',', objs)}");
    /// <summary>
    /// Print the 'Stopwatch.Elapsed' and restart the stopwatch.
    /// </summary>
    /// <param name="stopwatch"></param>
    /// <param name="sector"></param>
    [Conditional("DEBUG")]
    public static void Lap(Stopwatch stopwatch, string sector)
    {
        PrintColor($"@ {sector} took: {stopwatch.Elapsed}", stopwatch.Elapsed.Seconds < 1 ? PRINT_COLOR.GREEN : PRINT_COLOR.YELLOW);
        stopwatch.Restart();
    }
    private static string Timing() => timing.Elapsed.TotalSeconds.ToString("0000.00000");
}