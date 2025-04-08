using Godot;

namespace Support.Diagnostics;

public static class ConsoleLogWriter
{
    public enum PRINT_COLOR
    {
        DEFAULT, BLACK, RED, GREEN, YELLOW, BLUE, MAGENTA, PINK, PURPLE, CYAN, WHITE, ORANGE, GRAY
    }
    public static void Print(string msg, PRINT_COLOR color = PRINT_COLOR.DEFAULT)
    {
        if (color == PRINT_COLOR.DEFAULT) { GD.Print(msg); }
        else { GD.PrintRich($"[color={color.ToString().ToLower()}]{msg}[/color]"); }
    }
    public static void PrintWarn(string msg) => GD.PushWarning(msg);
}