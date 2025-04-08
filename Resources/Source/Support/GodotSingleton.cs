using Godot;

namespace Support;

/// <summary>
/// Implement a simple version of singleton pattern.
/// </summary>
/// <typeparam name="TSelf">Expect the derivated class to be accessed by the Instance prop.</typeparam>
public partial class GodotSingleton<TSelf> : Node where TSelf : class
{
    public static TSelf? Instance { get; private set; }
    public override void _EnterTree() => Instance = this as TSelf;
    public override void _ExitTree() => Instance = null;
}