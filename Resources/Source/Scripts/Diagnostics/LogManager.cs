using System.Collections.Generic;
using Godot;
using Support.Diagnostics;

namespace Support.Scripts.Diagnostics;

public partial class LogManager : GodotSingletonNode<LogManager>
{
    private static DummyLogger? dummyLogger;
    private static FileLogWriter? fileWriter;
    [Export] private LoggerSettings? defaultSettings;
    [Export] private Godot.Collections.Dictionary<string, LoggerSettings?>? settings;
    public override void _Ready()
    {
        dummyLogger = new DummyLogger();
        fileWriter = new FileLogWriter();
    }
    public ILogger CreateLogger(object obj) => CreateLogger(obj.GetType().Name);
    public ILogger CreateLogger(string name, LoggerSettings? loggerSettings = null)
    {
        if (loggerSettings is null && IsDummy(name, out loggerSettings))
        {
            return dummyLogger!;
        }
        else
        {
            return new Logger(name, loggerSettings!, fileWriter);
        }
    }
    /// <summary>
    /// Check if settings exists for given log name and if it would print anything.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="loggerSettings"></param>
    /// <returns>true if it should use the empty logger, dummy logger.</returns>
    private bool IsDummy(string name, out LoggerSettings? loggerSettings)
    {
        loggerSettings = settings?.GetValueOrDefault(name, null) ?? defaultSettings;
        if (loggerSettings is not null)
        {
            return loggerSettings!.Level == E_LOG_LEVEL.NONE || loggerSettings.Type == E_LOG_TYPE.NONE;
        }
        return true;
    }
}
