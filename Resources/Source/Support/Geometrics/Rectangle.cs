using System.Numerics;
using System.Runtime.CompilerServices;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a rectangle position and size in 2D space.
/// By default: min is top left, max is bottom right (Godot).
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Rectangle<N> : IConstraintable where N : INumber<N>
{
    /// <summary>
    /// Top Left position.
    /// </summary>
    public Vec2<N> min;
    private Vec2<N> _size;
    /// <summary>
    /// Bottom Right position.
    /// </summary>
    public Vec2<N> Max
    {
        readonly get => min + Size;
        set => Size = value - min;
    }
    public Vec2<N> Center
    {
        readonly get => Size / N.CreateChecked(2) + min;
        set => min = value - Size / N.CreateChecked(2);
    }
    /// <summary>
    /// Always positive. Negative values become zero.
    /// </summary>
    public Vec2<N> Size
    {
        readonly get => _size;
        set { _size = value; EnforceConstraint(); }
    }
    public Rectangle(in Vec2<N> min, in Vec2<N> size)
    {
        this.min = min;
        _size = size;
        EnforceConstraint();
    }
    public void EnforceConstraint()
    {
        var zero = N.CreateChecked(0);
        if (_size.x < zero) { _size.x = zero; }
        if (_size.y < zero) { _size.y = zero; }
    }
    public void GrowInPlace(in Vec2<N> grow)
    {
        var center = Center;
        Size += grow;
        Center = center;
    }
    public readonly bool IsPointIn(in Vec2<N> point) => (point >= min).AllTrue && (point <= Max).AllTrue;
    public readonly Vec2<N> Clamp(Vec2<N> point)
    {
        var max = Max;
        return new(N.Clamp(point.x, min.x, max.x), N.Clamp(point.y, min.y, max.y));
    }
    public void Encapsulates(in Vec2<N> point)
    {
        if (IsPointIn(point)) { return; }
        var max = Max;
        if (point.x < min.x) { min.x = point.x; }
        else if (point.x > max.x) { max.x = point.x; }
        if (point.y < min.y) { min.y = point.y; }
        else if (point.y > max.y) { max.y = point.y; }
        Max = max;
    }
    public readonly Vec2<N> LinearInterpolation(in Vec2<float> uv)
    {
        return min + (Size.CastTo<float>() * uv).CastTo<N>();
    }
    public readonly Vec2<float> InverseLinearInterpolation(in Vec2<N> point)
    {
        return (point - min).CastTo<float>() / Size.CastTo<float>();
    }
    #region CONVERTERS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Rectangle<TOutput> ConvertTo<TOutput>() where TOutput : INumber<TOutput> => Rectangle<TOutput>.CreateFrom(min, Size);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<N> CreateFrom<TInput>(Vec2<TInput> min, Vec2<TInput> size)
        where TInput : INumber<TInput> => new(min.CastTo<N>(), size.CastTo<N>());
    #endregion CONVERTERS
}