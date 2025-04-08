using System.Diagnostics;

namespace Support.Diagnostics;

public class DummyLogger : ILogger
{
    public void Debug(params object[] objs) { }
    public void Error(params object[] objs) { }
    public void Info(params object[] objs) { }
    public void Warn(params object[] objs) { }
    public void Lap(Stopwatch stopwatch, string sector) { }
    public void Dispose() { }
}
