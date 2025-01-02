using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a line segment with begining and end in 3d space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Segment3<N> where N : INumber<N>
{
    public Vec3<N> start;
    public Vec3<N> end;
    public readonly Vec3<N> Direction => end - start;
    public readonly Segment3<N> Inverted => new() { start = end, end = start };
    public Segment3(in Vec3<N> start, in Vec3<N> end)
    {
        this.start = start;
        this.end = end;
    }
}
