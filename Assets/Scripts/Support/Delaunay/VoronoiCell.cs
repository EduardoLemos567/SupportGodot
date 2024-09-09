using System;
using System.Collections.Generic;
using System.Linq;
using Support.Diagnostics;
using Support.Geometrics;
using Support.Numerics;

namespace Support.Delaunay;

public struct VoronoiCell
{
    public const int MIN_POINTS = 2;
    private IdentifiedPoint center;
    private IdentifiedPoint[] border;
    public readonly IdentifiedPoint Center => center;
    /// <summary>
    /// Count of border points that form this cell.
    /// </summary>
    public readonly int BorderPointsCount => border.Length;
    /// <summary>
    /// Ids of the adjacent voronoi cells. Not all cell edges will have a neighbor cell,
    /// such is the case of border cells.
    /// </summary>
    public readonly IEnumerable<int> AdjacentIds => from p in border where p.HasAdjacent select p.id;
    /// <summary>
    /// Ordered points that compose the cell border.
    /// </summary>
    public readonly IEnumerable<Vec2<double>> BorderPoints => from p in border select p.point;
    /// <summary>
    /// A closed cell means all border points surround the center.
    /// Only border cells can start opened. Inner cells are always closed.
    /// All cells should be closed, but some cells start open
    /// and we close later on VoronoiCellGenerator. 
    /// </summary>
    public bool IsClosed { get; private set; }
    public readonly int TrianglesCount => border.Length - (IsClosed ? 0 : 1);
    public VoronoiCell(in IdentifiedPoint center, IEnumerable<IdentifiedPoint> border, bool isClosed)
    {
        this.center = center;
        this.border = border.ToArray();
        Debug.Assert(this.border.Length >= MIN_POINTS, $"points.Length is less than {MIN_POINTS}, not enough to return a triangle");
        IsClosed = isClosed;
    }
    public readonly IEnumerable<Triangle2<double>> GetTriangles()
    {
        var last = border.Length - 1;
        for (var i = 0; i < last; i++)
        {
            yield return new() { A = border[i].point, B = center.point, C = border[i + 1].point };
        }
        if (IsClosed)
        {
            yield return new() { A = border[last].point, B = center.point, C = border[0].point };
        }
    }
    /// <summary>
    /// Close cells by inserting enough points to surround the center. 
    /// Points are inserted based on the 'direction' on the border this cell is on and the 'borderBounds'.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="borderBounds"></param>
    public void MakeClosedUsingBorders(in Direction direction, in Rectangle<double> borderBounds)
    {
        /*
        
        Non Diagonal
        ____
        |\/|    +3 triangles, +2 points
        |/\|

        Diagonal

         / \
        / | \
        |\|/|   +4 triangles, +3 points
        |/ \|
         
         */
        if (IsClosed) { return; }
        // Resize
        var temp = new IdentifiedPoint[border.Length + (direction.IsDiagonal ? 3 : 2)];
        Array.Copy(border, 0, temp, 0, border.Length);
        var last = border.Length - 1;
        //var calculatedOffset = borderBounds.Clamp(direction.ToVec2<double>() * double.PositiveInfinity);
        var infinite = direction.ToFloat().CastTo<double>() * double.PositiveInfinity;
        // Apply calculated offset
        if (direction.IsDiagonal)
        {
            switch (direction.AsEnum)
            {
                case Direction.DIRECTIONS.UP_RIGHT:
                    temp[last + 1].point = borderBounds.Clamp(border[last].point + new Vec2<double>(infinite.x, 0));   // right
                    temp[last + 3].point = borderBounds.Clamp(border[0].point + new Vec2<double>(0, infinite.y)); // up
                    break;
                case Direction.DIRECTIONS.DOWN_RIGHT:
                    temp[last + 1].point = borderBounds.Clamp(border[last].point + new Vec2<double>(0, infinite.y));   // down
                    temp[last + 3].point = borderBounds.Clamp(border[0].point + new Vec2<double>(infinite.x, 0)); // right
                    break;
                case Direction.DIRECTIONS.DOWN_LEFT:
                    temp[last + 1].point = borderBounds.Clamp(border[last].point + new Vec2<double>(infinite.x, 0));   // left
                    temp[last + 3].point = borderBounds.Clamp(border[0].point + new Vec2<double>(0, infinite.y)); // down
                    break;
                case Direction.DIRECTIONS.UP_LEFT:
                    temp[last + 1].point = borderBounds.Clamp(border[last].point + new Vec2<double>(0, infinite.y));   // up
                    temp[last + 3].point = borderBounds.Clamp(border[0].point + new Vec2<double>(infinite.x, 0)); // left
                    break;
            }
            temp[last + 2].point = borderBounds.Clamp(center.point + infinite);
        }
        else
        {
            if (direction.IsHorizontal)
            {
                infinite.y = 0;
            }
            else
            {
                infinite.x = 0;
            }
            temp[last + 1].point = borderBounds.Clamp(border[last].point + infinite);
            temp[last + 2].point = borderBounds.Clamp(border[0].point + infinite);
        }
        // Save and consider closed
        border = temp;
        IsClosed = true;
    }
    public override readonly string ToString() => $"VoronoiCell(center: {Center.point}, border: [{string.Join(',', border)}])";
}
