using Godot;

namespace Support.Diagnostics;

[GlobalClass]
public partial class LoggerSettings : Resource
{
    [Export] public E_LOG_LEVEL Level { get; private set; }
    [Export] public E_LOG_TYPE Type { get; private set; }
}
