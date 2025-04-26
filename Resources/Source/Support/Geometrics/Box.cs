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
        var zero = N.CreateTruncating(0);
        if (_size.x < zero) { _size.x = zero; }
        if (_size.y < zero) { _size.y = zero; }
        if (_size.z < zero) { _size.z = zero; }
    }
    public readonly bool IsPointIn(in Vec3<N> point) => (point >= min).AllTrue && (point <= Max).AllTrue;
    public readonly bool IsBoxIn(in Box<N> other) => IsPointIn(other.min) && IsPointIn(other.Max);
    //TODO: test this, prolly is wrong.
    public readonly bool IsBoxContact(in Box<N> other) => IsPointIn(other.min) || IsPointIn(other.Max);
    public readonly Vec3<N> Clamp(in Vec3<N> value) => value.Clamp(min, Max);
    public readonly Vec3<N> Lerp(in Vec3<N> value) => (value - min) / (Max - min);
    public readonly Vec3<N> InverseLerp(in Vec3<N> value) => value * (Max - min) + min;
    //TODO: create methods for inflate for box or point
}
