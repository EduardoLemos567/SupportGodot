using System.Numerics;
using System.Runtime.CompilerServices;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a rectangle position and size in 2D space.
/// By default: min is top left, max is bottom right (Godot).
/// Its constrained to have Min value as always min, if max value becomes the min, 
/// its values are shifted to always form a rectangle with positive size.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Rectangle<N> : IConstraintable where N : INumber<N>
{
    public static readonly Rectangle<N> One = new(Vec2<N>.Zero, Vec2<N>.One);
    private static readonly N TWO = N.CreateTruncating(2);
    private Vec2<N> _min;
    private Vec2<N> _max;
    /// <summary>
    /// Top Left position, changes translate the rectangle and changes its size too.
    /// </summary>
    public Vec2<N> Min
    {
        readonly get => _min;
        set { _min = value; EnforceConstraint(); }
    }
    /// <summary>
    /// Bottom Right position, changes goes into size alone.
    /// </summary>
    public Vec2<N> Max
    {
        readonly get => _max;
        set { _max = value; EnforceConstraint(); }
    }
    /// <summary>
    /// Position of the rectangle, changes translate the rectangle as whole.
    /// </summary>
    public Vec2<N> Position
    {
        readonly get => _min;
        set
        {
            var size = Size;
            _min = value;
            _max = value + size;
        }
    }
    /// <summary>
    /// Center position, changes become translation of the rectangle.
    /// </summary>
    public Vec2<N> Center
    {
        readonly get => Size / TWO + _min;
        set
        {
            var offset = Size / TWO;
            _min = value - offset;
            _max = value + offset;
        }
    }
    /// <summary>
    /// Always positive.
    /// </summary>
    public Vec2<N> Size
    {
        readonly get => _max - _min;
        set
        {
            _max = _min + value;
            EnforceConstraint();
        }
    }
    /// <summary>
    /// Calculate the distance between the center and the sides. Its half of the size.
    /// </summary>
    public Vec2<N> Radius
    {
        readonly get => Size / TWO;
        set
        {
            var center = Center;
            _min = center - value;
            _max = center + value;
        }
    }
    public Rectangle(in Vec2<N> min, in Vec2<N> size)
    {
        _min = min;
        Size = size;
        EnforceConstraint();
    }
    public void EnforceConstraint()
    {
        _min = _min.Min(_max);
        _max = _max.Max(_min);
    }
    public readonly bool IsPointIn(in Vec2<N> point) => (point >= _min).AllTrue && (point <= _max).AllTrue;
    public readonly Vec2<N> Clamp(in Vec2<N> point) => point.Clamp(_min, _max);
    /// <summary>
    /// Resize the box to encapsulate the given point.
    /// </summary>
    /// <param name="point"></param>
    public void Encapsulates(in Vec2<N> point)
    {
        _min = _min.Min(point);
        _max = _max.Max(point);
    }
    public readonly Vec2<N> Lerp(in Vec2<float> uv)
    {
        return _min + (Size.CastTo<float>() * uv).CastTo<N>();
    }
    public readonly Vec2<float> InverseLerp(in Vec2<N> point)
    {
        return (point - _min).CastTo<float>() / Size.CastTo<float>();
    }
    public readonly Vec2<N> PositiveModulo(in Vec2<N> point)
    {
        return new(
            Toolbox.PositiveModulo(point.x, Size.x),
            Toolbox.PositiveModulo(point.y, Size.y));
    }
    public readonly Vec2<int> CompareTo(in Vec2<N> point)
    {
        var result = Vec2<int>.Zero;

        if (point.x < _min.x) { result.x = -1; }
        else if (point.x > _max.x) { result.x = 1; }
        if (point.y < _min.y) { result.y = -1; }
        else if (point.y > _max.y) { result.y = 1; }

        return result;
    }
    public readonly Direction PointOutsideDirection(in Vec2<N> point) => Direction.FromDelta(CompareTo(point));
    #region CONVERTERS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Rectangle<TOutput> CastTo<TOutput>() where TOutput : INumber<TOutput> => Rectangle<TOutput>.CreateFrom(_min, Size);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rectangle<N> CreateFrom<TInput>(Vec2<TInput> min, Vec2<TInput> size)
        where TInput : INumber<TInput> => new(min.CastTo<N>(), size.CastTo<N>());
    #endregion CONVERTERS
    public override readonly string ToString() => $"(min: {_min}, Size: {Size})";
}