using System.Collections.Generic;
using System.Numerics;
using Support.Numerics;

namespace Support.Geometrics.Hexagons.Pointy;

public class Grid<F> where F : IFloatingPoint<F>
{
    public Vec2<int> Count { get; }
    public Vec2<F> HexagonSize { get; }
    public F HexagonHalfWidth { get; }
    public F HexagonQuarterHeight { get; }
    private Vec2<F> usedOffset;
    public Rectangle<F> Bounds { get; }
    public IReadOnlyList<Vec2<F>> PointOffsets { get; }
    public Grid(F hexagonRadius, Vec2<int> count) : this(HexagonSizeFromRadius(hexagonRadius), count) { }
    public Grid(Vec2<F> hexagonSize, Vec2<int> count)
    {
        HexagonSize = hexagonSize;
        Count = count;
        HexagonHalfWidth = HexagonSize.x / F.CreateChecked(2);
        HexagonQuarterHeight = HexagonSize.y / F.CreateChecked(4);
        usedOffset = new Vec2<F>(HexagonHalfWidth, HexagonQuarterHeight * F.CreateChecked(2));
        var s = new Vec2<F>(
            F.CreateChecked(2 * Count.x) * HexagonHalfWidth + (Count.y > 1 ? HexagonHalfWidth : F.CreateChecked(0)),
            F.CreateChecked(3 * Count.y) * HexagonQuarterHeight + HexagonQuarterHeight);    //TODO: test
        Bounds = new() { Size = s };
        {
            var po = new Vec2<F>[6];
            po[(int)E_POINT.UP] = new(F.CreateChecked(0), -HexagonQuarterHeight * F.CreateChecked(2));
            po[(int)E_POINT.UP_RIGHT] = new(HexagonHalfWidth, -HexagonQuarterHeight);
            po[(int)E_POINT.DOWN_RIGHT] = new(HexagonHalfWidth, HexagonQuarterHeight);
            po[(int)E_POINT.DOWN] = -po[(int)E_POINT.UP];
            po[(int)E_POINT.DOWN_LEFT] = -po[(int)E_POINT.UP_RIGHT];
            po[(int)E_POINT.UP_LEFT] = -po[(int)E_POINT.DOWN_RIGHT];
            PointOffsets = po;
        }
    }
    public Hexagon<F> GetHexagon(in Coordinate coordinate) => new(GetCenterPoint(coordinate), PointOffsets);
    public Vec2<F> GetCenterPoint(in Coordinate coordinate)
    {
        var c = coordinate.offset.CastTo<F>();
        // If odd: offset of a halfwidth
        var oddOffset = Toolbox.IsEven(coordinate.offset.y) ? F.CreateChecked(0) : HexagonHalfWidth;
        // offset + width * coord.x, 3 * quarter height * coord.y)
        return usedOffset + new Vec2<F>(oddOffset + HexagonSize.x * c.x, F.CreateChecked(3) * HexagonQuarterHeight * c.y);
    }
    public Coordinate FindCoordinate(in Vec2<F> position)
    {
        var toGridPosition = position;
        int y = int.CreateChecked(F.Floor(toGridPosition.y / HexagonQuarterHeight));
        var (yResult, yRemains) = int.DivRem(y, 3);
        if (!Toolbox.IsEven(yResult)) { toGridPosition.x -= HexagonHalfWidth; }
        int x = int.CreateChecked(F.Floor(toGridPosition.x / HexagonSize.x));
        var coordinate = new Coordinate(new(x, yResult));
        if (yRemains == 0)
        {   // top triangle slice
            var c = GetCenterPoint(coordinate);
            var v = c + PointOffsets[(int)E_POINT.UP];
            Segment2<F> s;
            if (position.x <= c.x)
            {   // left slice
                s = new(v, c + PointOffsets[(int)E_POINT.UP_LEFT]);
                if (s.IsPointRight(position))
                {   // out current hex
                    coordinate.Translate(E_TRANSLATION.UP_LEFT);
                }
            }
            else
            {   // right slice
                s = new(v, c + PointOffsets[(int)E_POINT.UP_RIGHT]);
                if (!s.IsPointRight(position))
                {   // out current hex
                    coordinate.Translate(E_TRANSLATION.UP_RIGHT);
                }
            }
        }
        return coordinate;
    }
    public bool IsCoordinateValid(in Coordinate coordinate) =>
        (coordinate.offset >= Vec2<int>.Zero).AllTrue &&
        (coordinate.offset < Count).AllTrue;
    public IEnumerable<Coordinate> PlotCoordinates()
    {
        var c = new Coordinate();
        for (c.offset.y = 0; c.offset.y < Count.y; c.offset.y++)
        {
            for (c.offset.x = 0; c.offset.x < Count.x; c.offset.x++)
            {
                yield return c;
            }
        }
    }
    private static Vec2<F> HexagonSizeFromRadius(F radius) => new(radius * F.CreateChecked(Toolbox.SQR3), radius * F.CreateChecked(2));
}
