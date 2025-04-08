using System;
using System.Diagnostics;

namespace Support.Diagnostics;

public interface ILogger : IDisposable
{
    void Error(params object[] objs);
    void Warn(params object[] objs);
    void Info(params object[] objs);
    void Debug(params object[] objs);
    void Lap(Stopwatch stopwatch, string sector);
}
