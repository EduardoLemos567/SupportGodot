using Godot;
using Support.Diagnostics;

namespace Support.Scripts.Diagnostics;

[GlobalClass]
public partial class LoggerSettings : Resource, ILoggerSettings
{
    [Export] public E_LOG_LEVEL Level { get; private set; }
    [Export] public E_LOG_TYPE Type { get; private set; }
}
