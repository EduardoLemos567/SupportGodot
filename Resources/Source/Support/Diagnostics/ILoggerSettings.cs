namespace Support.Diagnostics;

public interface ILoggerSettings
{
    E_LOG_LEVEL Level { get; }
    E_LOG_TYPE Type { get; }
}