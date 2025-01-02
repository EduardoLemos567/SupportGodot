using Godot;
using Godot.Collections;

namespace Support.Diagnostics;

public partial class LogManager : GodotSingleton<LogManager>
{
    [Export] private Dictionary<string, bool>? relevance;
    public bool IsRelevant(string name)
    {
        return relevance is not null &&
               relevance.ContainsKey(name) &&
               relevance[name];
    }
    public void Log(string name, object msg)
    {
        if (IsRelevant(name))
        { Debug.Print($"[{name}] {msg}"); }
    }
    public void Log(string name, string place, object msg)
    {
        if (IsRelevant(name))
        { Debug.Print($"[{name}:{place}] {msg}"); }
    }
    public void Log(string name, object obj, object msg)
    {
        if (IsRelevant(name))
        { Debug.Print($"[{name}:{obj.GetType().Name}({obj.GetHashCode()})] {msg}"); }
    }
}