using GodotColor = Godot.Color;

namespace Support;

public static class ColorExtensions
{
    public static Color ToColor(in this GodotColor from) => new(from.R, from.G, from.B, from.A);
    public static GodotColor ToGodotColor(in this Color from) => new(from.r, from.g, from.b, from.a);
}