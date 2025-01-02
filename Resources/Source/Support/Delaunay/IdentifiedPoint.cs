using Support.Numerics;

namespace Support.Delaunay
{
    /// <summary>
    /// A tuple of point and id. Its used in two known ways:
    /// <br>- Describes the center point of a voronoi cell and its id </br>
    /// <br>- Describer the border point of a cell and the center id of the neighbor cell.</br>
    /// <br>In this case it may or may not have a neighbor cell, such is the case of border cells.</br>
    /// <br>In this case the id -1, meaning its a border cell with no neighbor.</br>
    /// </summary>
    public struct IdentifiedPoint
    {
        public Vec2<double> point;
        public int id;
        public readonly bool HasAdjacent => id != -1;
        public IdentifiedPoint(Vec2<double> point, int id)
        {
            this.point = point;
            this.id = id;
        }
        public override readonly string ToString() => $"({point}, {(HasAdjacent ? id : "null")})";
    }
}
