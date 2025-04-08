using System.Diagnostics;

namespace Support.Diagnostics;

public class Logger : ILogger
{
    private static readonly Stopwatch beginClock = Stopwatch.StartNew();
    private readonly string name;
    private readonly LoggerSettings settings;
    private readonly FileLogWriter? fileLogger;
    public Logger(string name, LoggerSettings settings, FileLogWriter? fileLogger)
    {
        this.name = name;
        this.settings = settings;
        this.fileLogger = fileLogger;
    }
    public void Error(params object[] objs) => Print(E_LOG_LEVEL.ERROR, objs);
    public void Warn(params object[] objs) => Print(E_LOG_LEVEL.WARNING, objs);
    public void Info(params object[] objs) => Print(E_LOG_LEVEL.INFO, objs);
    public void Debug(params object[] objs) => Print(E_LOG_LEVEL.DEBUG, objs);
    /// <summary>
    /// Print the 'Stopwatch.Elapsed' and restart the stopwatch.
    /// </summary>
    /// <param name="stopwatch"></param>
    /// <param name="sector"></param>
    public void Lap(Stopwatch stopwatch, string sector)
    {
        if (!settings.Level.HasFlag(E_LOG_LEVEL.LAP)) { return; }
        var msg = FormatMessage($"@ {sector} took: {stopwatch.Elapsed}");
        var color = stopwatch.Elapsed.Seconds < 1 ? ConsoleLogWriter.PRINT_COLOR.GREEN : ConsoleLogWriter.PRINT_COLOR.YELLOW;
        if (settings.Type.HasFlag(E_LOG_TYPE.CONSOLE))
        {
            ConsoleLogWriter.Print(msg, color);
        }
        stopwatch.Restart();
    }
    public void Dispose() { }
    private void Print(E_LOG_LEVEL level, params object[] objs)
    {
        if (!settings.Level.HasFlag(level)) { return; }
        var msg = FormatMessage(objs);
        var color = level switch
        {
            E_LOG_LEVEL.ERROR => ConsoleLogWriter.PRINT_COLOR.RED,
            E_LOG_LEVEL.WARNING => ConsoleLogWriter.PRINT_COLOR.YELLOW,
            E_LOG_LEVEL.INFO => ConsoleLogWriter.PRINT_COLOR.CYAN,
            E_LOG_LEVEL.DEBUG => ConsoleLogWriter.PRINT_COLOR.GREEN,
            _ => ConsoleLogWriter.PRINT_COLOR.DEFAULT
        };
        if (settings.Type.HasFlag(E_LOG_TYPE.CONSOLE))
        {
            ConsoleLogWriter.Print(msg, color);
        }
    }
    private string FormatMessage(params object[] objs)
    {
        return $"[{Timing()}][{name}]{string.Join(',', objs)}";
    }
    private static string Timing()
    {
        return beginClock.Elapsed.TotalSeconds.ToString("0000.00000");
    }
}
