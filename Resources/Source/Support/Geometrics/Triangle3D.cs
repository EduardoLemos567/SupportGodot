using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a triangle positions in 3D space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Triangle3D<N> where N : INumber<N>
{
    public Vec3<N> A;
    public Vec3<N> B;
    public Vec3<N> C;
    public Segment3D<N> AB => new() { start = A, end = B };
    public Segment3D<N> BC => new() { start = B, end = C };
    public Segment3D<N> CA => new() { start = C, end = A };
    public Triangle3D(in Vec3<N> a, in Vec3<N> b, in Vec3<N> c)
    {
        A = a;
        B = b;
        C = c;
    }
}
