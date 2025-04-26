using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics;

/// <summary>
/// This will represent a line segment with begining and end in 2d space.
/// </summary>
/// <typeparam name="N"></typeparam>
public struct Segment2<N> where N : INumber<N>
{
    public Vec2<N> start;
    public Vec2<N> end;
    public readonly Vec2<N> Direction => end - start;
    public readonly Segment2<N> Inverted => new() { start = end, end = start };
    public Segment2(in Vec2<N> start, in Vec2<N> end)
    {
        this.start = start;
        this.end = end;
    }
    /// <summary>
    /// Threat the segment as line (infinity segment) and check if given point is at right position
    /// of the line created by the direction of start->end.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>true if is right or at the line, false if is at left.</returns>
    public readonly bool IsPointRight(in Vec2<N> point)
    {
        var d = Direction;
        var p = point - start;
        return (d.x * p.y - d.y * p.x) >= N.CreateTruncating(0);
    }
}
