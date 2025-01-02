using Support.Geometrics;
using Support.Numerics;

namespace Support.Delaunay
{
    public partial class ClosedVoronoiCellsGenerator
    {
        public struct Parameters
        {
            /// <summary>
            /// Start with a grid of points
            /// </summary>
            public Vec2<int> gridRepeat;
            /// <summary>
            /// Control percent of points to use
            /// </summary>
            public float percentGridPoints;
            /// <summary>
            /// Real world size of the final rect
            /// </summary>
            public Rectangle<double> gridRect;
            /// <summary>
            /// Number of times to repeat centroid relaxation of the points
            /// </summary>
            public int repeatRelax;
            /// <summary>
            /// Additional border distance to create straigh edges on the map border
            /// </summary>
            public float borderDistance;
            /// <summary>
            /// Shrink ValidBounds zone percentage adding ids into BorderIds.
            /// </summary>
            public float borderExtra;
        }
    }
}