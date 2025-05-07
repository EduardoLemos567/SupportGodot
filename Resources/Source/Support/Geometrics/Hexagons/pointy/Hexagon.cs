using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics.Hexagons.Pointy;

public enum E_POINT : byte
{
    UP,
    UP_RIGHT,
    DOWN_RIGHT,
    DOWN,
    DOWN_LEFT,
    UP_LEFT
}

public readonly struct Hexagon<F> where F : IFloatingPoint<F>
{
    public readonly Vec2<F> center;
    public readonly IReadOnlyList<Vec2<F>> pointOffsets;
    public IEnumerable<Vec2<F>> BorderPoints
    {
        get
        {
            foreach (var point in pointOffsets)
            {
                yield return center + point;
            }
        }
    }
    public Hexagon(in Vec2<F> center, in IReadOnlyList<Vec2<F>> pointOffsets)
    {
        this.center = center;
        Debug.Assert(pointOffsets.Count == 6, "Hexagon need pointOffsets[6]");
        this.pointOffsets = pointOffsets;
    }
    public readonly Vec2<F> GetBorderPoint(in E_POINT point) => center + pointOffsets[(int)point];
    public bool IsPointIn(in Vec2<F> point)
    {
        Segment2D<F> s;
        Vec2<F> h, v;
        if (point.y <= center.y)
        {   // upper slice
            v = GetBorderPoint(E_POINT.UP);
            if (point.x >= center.x)
            {   // right slice
                h = GetBorderPoint(E_POINT.UP_RIGHT);
                s = new(v, h);
                return s.IsPointRight(point) && point.x <= h.x;
            }
            else
            {   // left slice
                h = GetBorderPoint(E_POINT.UP_LEFT);
                s = new(v, h);
                return !s.IsPointRight(point) && point.x >= h.x;
            }
        }
        else
        {   // lower slice
            v = GetBorderPoint(E_POINT.DOWN);
            if (point.x >= center.x)
            {   // right slice
                h = GetBorderPoint(E_POINT.DOWN_RIGHT);
                s = new(v, h);
                return !s.IsPointRight(point) && point.x <= h.x;
            }
            else
            {   // left slice
                h = GetBorderPoint(E_POINT.DOWN_LEFT);
                s = new(v, h);
                return s.IsPointRight(point) && point.x >= h.x;
            }
        }

    }
}
