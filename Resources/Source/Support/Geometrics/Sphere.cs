using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a sphere position and radius in 3d space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Sphere<N> : IConstraintable where N : INumber<N>
{
    private N _radius;
    public N Radius
    {
        readonly get => _radius;
        set { _radius = value; EnforceConstraint(); }
    }
    public Vec3<N> center;
    public Sphere(in Vec3<N> center, N radius)
    {
        this.center = center;
        _radius = radius; EnforceConstraint();
    }
    public void EnforceConstraint()
    {
        if (_radius < N.Zero) { _radius = N.Zero; }
    }
}
