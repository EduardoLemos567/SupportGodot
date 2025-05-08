using System.Collections.Generic;
using System.Linq;
using Godot;
using Support.Geometrics;
using Support.Numerics;
using Support.Rng;

namespace Support.Delaunay
{
    /// <summary>
    /// Generates closed voronoi cells. Closed voronoi cells are cells that we ensured that can be draw correctly using triangles.
    /// Normally the voronoi cells output by the triangulator will have border cells where the center is exposed as a border point.
    /// By adding some points around that center we enclose it, allowing it to be draw correctly in a rectangular mesh.
    /// It generates on creation.
    /// </summary>
    public partial class ClosedVoronoiCellsGenerator
    {
        /// <summary>
        /// All generated closed VoronoiCells
        /// </summary>
        public IReadOnlyList<VoronoiCell> Cells { get; private set; }
        /// <summary>
        /// The indices of all VoronoiCells in the list which were added as borders.
        /// </summary>
        public ReadOnlySet<int> BorderIds { get; private set; }
        /// <summary>
        /// Generated bounds based on parameters
        /// </summary>
        public Rectangle<double> Bounds { get; private set; }
        /// <summary>
        /// Bounds where all points that should not be in border were added
        /// </summary>
        public Rectangle<double> ValidBounds { get; private set; }
        public int TriangleCount => CalculateTriangleCount(Enumerable.Range(0, Cells.Count));
        /// <summary>
        /// Generate on creation.
        /// </summary>
        /// <param name="rng"></param>
        /// <param name="parameters"></param>
        public ClosedVoronoiCellsGenerator(in ARng rng, in Parameters parameters)
        {
            var bounds = parameters.gridRect;
            var points = GeneratePoints(rng, parameters, bounds);
            var borderDirections = IncludeBorder(parameters, ref bounds, points);
            var triangulator = new Triangulator();
            triangulator.SetPoints(points);
            for (var i = 0; i < parameters.repeatRelax; i++)
            {
                triangulator.Relax();
            }
            Cells = CollectVoronoiCells(parameters, bounds, triangulator, borderDirections);
            var borderIds = new HashSet<int>(borderDirections.Keys);
            ValidBounds = AddExtraBorderIds(parameters, bounds, Cells, borderIds);
            Bounds = bounds;
            BorderIds = borderIds;
        }
        public int CalculateTriangleCount(in IEnumerable<int> ids) => (from id in ids select Cells[id].TrianglesCount).Sum();
        private static List<Vec2<double>> GeneratePoints(in ARng rng, in Parameters parameters, in Rectangle<double> bounds)
        {
            var points = new List<Vec2<double>>(Mathf.RoundToInt(parameters.gridRepeat.x * parameters.gridRepeat.y * parameters.percentGridPoints));
            for (var i = 0; i < points.Capacity; i++)
            {
                points.Add(rng.GetPointIn(bounds));
            }
            return points;
        }
        private static Dictionary<int, Direction> IncludeBorder(in Parameters parameters, ref Rectangle<double> bounds, List<Vec2<double>> points)
        {
            var repeats = new Vec2<int>(
                Mathf.RoundToInt(parameters.gridRepeat.x * parameters.percentGridPoints),
                Mathf.RoundToInt(parameters.gridRepeat.y * parameters.percentGridPoints)
            );
            var newPoints = repeats.x * 2 + repeats.y * 2 + 4;
            var borderDirections = new Dictionary<int, Direction>(newPoints);
            points.Capacity += newPoints;
            var stride = bounds.Size / (repeats.CastTo<double>() - 1);
            for (var x = 0; x < repeats.x; x++)
            {   // Horizontal
                // Bottom
                points.Add(new(bounds.min.x + x * stride.x, bounds.min.y - parameters.borderDistance));
                borderDirections.Add(points.Count - 1, Direction.Down);
                // Top
                points.Add(new(bounds.min.x + x * stride.x, bounds.Max.y + parameters.borderDistance));
                borderDirections.Add(points.Count - 1, Direction.Up);
            }
            for (var y = 0; y < repeats.y; y++)
            {   // Vertical
                // Left
                points.Add(new(bounds.min.x - parameters.borderDistance, bounds.min.y + y * stride.y));
                borderDirections.Add(points.Count - 1, Direction.Left);
                // Right
                points.Add(new(bounds.Max.x + parameters.borderDistance, bounds.min.y + y * stride.y));
                borderDirections.Add(points.Count - 1, Direction.Right);
            }
            // Bottom Left
            points.Add(bounds.min - parameters.borderDistance);
            borderDirections.Add(points.Count - 1, Direction.DownLeft);
            // Bottom Right
            points.Add(new(bounds.Max.x + parameters.borderDistance, bounds.min.y - parameters.borderDistance));
            borderDirections.Add(points.Count - 1, Direction.DownRight);
            // Top Left
            points.Add(new Vec2<double>(bounds.min.x - parameters.borderDistance, bounds.Max.y + parameters.borderDistance));
            borderDirections.Add(points.Count - 1, Direction.UpLeft);
            // Top Right
            points.Add(bounds.Max + parameters.borderDistance);
            borderDirections.Add(points.Count - 1, Direction.UpRight);
            bounds.GrowInPlace(new(parameters.borderDistance * 2));
            return borderDirections;
        }
        private static IReadOnlyList<VoronoiCell> CollectVoronoiCells(in Parameters parameters, in Rectangle<double> bounds, in Triangulator triangulator, in Dictionary<int, Direction> borderDirections)
        {
            bounds.GrowInPlace(new(parameters.borderDistance * 2));
            var cells = new VoronoiCell[triangulator.Points.Count];
            foreach (var cell in triangulator.GetVoronoiCellsBasedOnCentroids())
            {
                if (!cell.IsClosed && borderDirections.TryGetValue(cell.Center.id, out var direction))
                {
                    cell.MakeClosedUsingBorders(direction, bounds);
                }
                cells[cell.Center.id] = cell;
            }
            return cells;
        }
        private static Rectangle<double> AddExtraBorderIds(in Parameters parameters, in Rectangle<double> bounds, in IReadOnlyList<VoronoiCell> cells, in HashSet<int> borderIds)
        {
            var validBounds = bounds;
            validBounds.GrowInPlace(-bounds.Size * parameters.borderExtra);
            if (parameters.borderExtra > 0)
            {
                foreach (var cell in cells)
                {
                    if (validBounds.IsPointIn(cell.Center.point)) { continue; }
                    _ = borderIds.Add(cell.Center.id);
                }
            }
            return validBounds;
        }
    }
}