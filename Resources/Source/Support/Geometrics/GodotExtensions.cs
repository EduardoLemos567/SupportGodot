using System.Numerics;
using System.Runtime.CompilerServices;
using Support.Numerics;

using GodotRect2 = Godot.Rect2;
using GodotRect2I = Godot.Rect2I;

namespace Support.Geometrics;

public static class GodotExtensions
{
    // GodotRect2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<N> ToRectangle<N>(in this GodotRect2 from) where N : INumber<N> => Rectangle<N>.CreateFrom(from.Position.ToVec2(), from.Size.ToVec2());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<float> ToRectangle(in this GodotRect2 from) => ToRectangle<float>(from);
    /// <summary>
    /// min is top left (min -> Position), max is bottom right (Max -> End).
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotRect2 ToGodotRect2(in this Rectangle<float> from) => new(from.min.ToGodotVector2(), from.Size.ToGodotVector2());
    // GodotRect2I
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<N> ToRectangle<N>(in this GodotRect2I from) where N : INumber<N> => Rectangle<N>.CreateFrom(from.Position.ToVec2(), from.Size.ToVec2());
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<int> ToRectangle(in this GodotRect2I from) => ToRectangle<int>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotRect2I ToGodotRect2I(in this Rectangle<int> from) => new(from.min.ToGodotVector2I(), from.Size.ToGodotVector2I());
}