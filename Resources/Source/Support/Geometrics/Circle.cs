using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a circle position and radius in 2D space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Circle<N> : IConstraintable where N : INumber<N>
{
    private N _radius;
    public N Radius
    {
        readonly get => _radius;
        set { _radius = value; EnforceConstraint(); }
    }
    public Vec2<N> center;
    public Circle(in Vec2<N> center, N radius) : this()
    {
        this.center = center;
        _radius = radius; EnforceConstraint();
    }
    public void EnforceConstraint()
    {
        var zero = N.CreateChecked(0);
        if (_radius < zero) { _radius = zero; }
    }
    public readonly Vec2<N> GetPointInCircumference(in RadianAngle radianAngle)
    {
        return center + (radianAngle.Delta * float.CreateChecked(Radius)).CastTo<N>();
    }
    public readonly bool IsPointIn(in Vec2<N> point) => center.SqrDistance(point) <= (Radius * Radius);
}
