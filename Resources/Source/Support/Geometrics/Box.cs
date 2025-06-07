using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a box position and size in 3D space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Box<N> : IConstraintable where N : INumber<N>
{
    public Vec3<N> min;
    private Vec3<N> _size;
    public Vec3<N> Max
    {
        readonly get => min + Size;
        set => Size = value - min;
    }
    public Vec3<N> Center
    {
        readonly get => Size / N.CreateTruncating(2) + min;
        set => min = value - Size / N.CreateTruncating(2);
    }
    public Vec3<N> Size
    {
        readonly get => _size;
        set { _size = value; EnforceConstraint(); }
    }
    public Box(in Vec3<N> min, in Vec3<N> size)
    {
        this.min = min;
        _size = size;
        EnforceConstraint();
    }
    public static Box<N> FromSphere(in Sphere<N> sphere)
    {
        return new(
            sphere.center - sphere.Radius,
            new(sphere.Radius * N.CreateTruncating(2)));
    }

    public void EnforceConstraint()
    {
        if (_size.x < N.Zero) { _size.x = N.Zero; }
        if (_size.y < N.Zero) { _size.y = N.Zero; }
        if (_size.z < N.Zero) { _size.z = N.Zero; }
    }
    public readonly bool IsPointIn(in Vec3<N> point) => (point >= min).AllTrue && (point <= Max).AllTrue;
    public readonly bool IsBoxIn(in Box<N> other) => IsPointIn(other.min) && IsPointIn(other.Max);
    public readonly bool IsBoxContact(in Box<N> other) => (other.Max >= min).AllTrue && (other.min <= Max).AllTrue;
    public readonly Vec3<N> Clamp(in Vec3<N> point) => point.Clamp(min, Max);
    public readonly Vec3<N> Lerp(in Vec3<N> uv) => (uv - min) / (Max - min);
    public readonly Vec3<N> InverseLerp(in Vec3<N> point) => point * (Max - min) + min;
    public readonly Vec3<N> PositiveModulo(in Vec3<N> point)
    {
        return new(
            Toolbox.PositiveModulo(point.x, Size.x),
            Toolbox.PositiveModulo(point.y, Size.y),
            Toolbox.PositiveModulo(point.z, Size.z));
    }
    /// <summary>
    /// Resize the rectangle to encapsulate the given point.
    /// </summary>
    /// <param name="point"></param>
    public void Encapsulates(in Vec3<N> point)
    {
        if (IsPointIn(point)) { return; }
        var max = Max;
        if (point.x < min.x) { min.x = point.x; }
        else if (point.x > max.x) { max.x = point.x; }
        if (point.y < min.y) { min.y = point.y; }
        else if (point.y > max.y) { max.y = point.y; }
        if (point.z < min.z) { min.z = point.z; }
        else if (point.z > max.z) { max.y = point.z; }
        Max = max;
    }
}
