using Support.Geometrics;
using Support.Numerics;

namespace Support.Delaunay
{
    public readonly struct DelaunayTriangle
    {
        /// <summary>
        ///      pB
        ///      /\
        ///   tA/  \tB
        /// pA /____\ pC
        ///      tC
        /// </summary>
        public readonly Vec2<double> A;
        public readonly Vec2<double> B;
        public readonly Vec2<double> C;
        public readonly int index;
        public readonly AdjacentTriangles adjacent;
        public DelaunayTriangle(in Triangle2<double> points,
                        int index,
                        in AdjacentTriangles adjacentTriangles)
        {
            A = points.A;
            B = points.B;
            C = points.C;
            this.index = index;
            adjacent = adjacentTriangles;
        }
        public static implicit operator Triangle2<float>(in DelaunayTriangle triangle) => new()
        {
            A = triangle.A.CastTo<float>(),
            B = triangle.B.CastTo<float>(),
            C = triangle.C.CastTo<float>()
        };
        public static implicit operator Triangle2<double>(in DelaunayTriangle triangle) => new()
        {
            A = triangle.A,
            B = triangle.B,
            C = triangle.C
        };

    }
}
