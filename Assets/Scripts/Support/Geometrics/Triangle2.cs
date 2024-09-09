using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a triangle positions in 2D space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Triangle2<N> where N : INumber<N>
{
    public Vec2<N> A;
    public Vec2<N> B;
    public Vec2<N> C;
    public readonly Segment2<N> AB => new() { start = A, end = B };
    public readonly Segment2<N> BC => new() { start = B, end = C };
    public readonly Segment2<N> CA => new() { start = C, end = A };
    public readonly bool IsClockwise => AB.IsPointRight(C) && BC.IsPointRight(A);
    public Triangle2(in Vec2<N> a, in Vec2<N> b, in Vec2<N> c)
    {
        A = a;
        B = b;
        C = c;
    }
    public readonly bool IsPointIn(in Vec2<N> point) =>
        AB.IsPointRight(point) &&
        BC.IsPointRight(point) &&
        CA.IsPointRight(point);
}
