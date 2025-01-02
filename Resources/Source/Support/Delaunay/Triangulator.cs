using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Support.Geometrics;
using Support.Numerics;

namespace Support.Delaunay;

public class Triangulator
{
    /// <summary>
    /// The initial points Triangulator was constructed with.
    /// </summary>
    public IReadOnlyList<Vec2<double>> Points => points!;
    /// <summary>
    /// Values are used to index the points, every 3 values forms a triangle.
    /// </summary>
    public IReadOnlyList<int> Triangles => triangles!;
    /// <summary>
    /// Values are used to/from index the Triangles.
    /// Values also are used to index itself for a opposite edge
    /// A index value from Triangles and into triangles index.
    /// </summary>
    public IReadOnlyList<int> HalfEdges => halfedges!;
    /// <summary>
    /// Values are used to index the points.
    /// Ordered for clockwise order of half-edges.
    /// </summary>
    public IReadOnlyList<int> Hull => hull!;
    private int[]? triangles;
    private int[]? halfedges;
    private int[]? hull;
    private Vec2<double>[]? points;
    private Vec2<double> graphCenter;
    private int trianglePointsCount;
    private int hashSize;
    private int hullStart;
    private int hullSize;
    // Temporary Arrays
    private int[]? tempHullNext;
    private int[]? tempHullPrev;
    private int[]? tempHullTri;
    private int[]? tempHullHash;
    private int[]? tempDistanceEdges;
    private double[]? tempDistanceValues;
    private int[]? tempEdgeStack;
    public int TrianglesCount => triangles!.Length / 3;
    /// <summary>
    /// No reallocation. Its safe as long you dont modify the input array.
    /// </summary>
    /// <param name="points"></param>
    public void SetPointsDirectly(Vec2<double>[] points)
    {
        this.points = points;
        Recalculate();
    }
    /// <summary>
    /// Pass a source of points. Array will be allocated and points will be copied into.
    /// </summary>
    /// <param name="points"></param>
    public void SetPoints(IEnumerable<Vec2<double>> points)
    {
        if (points is ICollection<Vec2<double>> collection)
        {
            if (this.points is null || this.points.Length != collection.Count)
            {
                this.points = new Vec2<double>[collection.Count];
            }
            collection.CopyTo(this.points, 0);
        }
        else
        {
            this.points = points.ToArray();
        }
        Recalculate();
    }
    public Rectangle<double> CalculateBounds()
    {
        if (points!.Length < 2)
        {
            throw new NotSupportedException("Need at least 2 points.");
        }
        var rect = new Rectangle<double>(points[0], Vec2<double>.Zero);
        for (var i = 1; i < points.Length; i++)
        {
            rect.Encapsulates(points[i]);
        }
        return rect;
    }
    private void Recalculate()
    {
        if (points!.Length < 3)
        {
            throw new NotSupportedException("Need at least 3 points");
        }
        ResizeTemporaryArrays();
        var maxPossibleTriangles = 2 * points.Length - 5;
        if (triangles is null || triangles.Length < maxPossibleTriangles * 3)
        {
            triangles = new int[maxPossibleTriangles * 3];
        }
        if (halfedges is null || halfedges.Length < maxPossibleTriangles * 3)
        {
            halfedges = new int[maxPossibleTriangles * 3];
        }
        graphCenter = CalculateBounds().Center;
        var minDist = double.PositiveInfinity;
        var seed1 = 0;
        var seed2 = 0;
        var seed3 = 0;
        // pick a seed point close to the center
        for (var i = 0; i < points.Length; i++)
        {
            var d = graphCenter.SqrDistance(points[i]);
            if (d < minDist)
            {
                seed1 = i;
                minDist = d;
            }
        }
        var seedPoint1 = points[seed1];
        minDist = double.PositiveInfinity;
        // find the point closest to the seed
        for (var pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            tempDistanceEdges![pointIndex] = pointIndex; // Fill the distanceEdges with Range(0, points.Length)
            if (pointIndex == seed1) { continue; }
            var d = graphCenter.SqrDistance(points[pointIndex]);
            if (d < minDist && d > 0)
            {
                seed2 = pointIndex;
                minDist = d;
            }
        }
        var seedPoint2 = points[seed2];
        var minRadius = double.PositiveInfinity;
        // find the third point which forms the smallest circumcircle with the first two
        for (var pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            if (pointIndex == seed1 || pointIndex == seed2) { continue; }
            var r = CalculateCircumRadius(seedPoint1, seedPoint2, points[pointIndex]);
            if (r < minRadius)
            {
                seed3 = pointIndex;
                minRadius = r;
            }
        }
        var seedPoint3 = points[seed3];
        if (minRadius == double.PositiveInfinity)
        {
            throw new Exception("No Delaunay triangulation exists for this input.");
        }
        if (NeedToReorient(seedPoint1, seedPoint2, seedPoint3))
        {
            (seed3, seed2) = (seed2, seed3);
            (seedPoint3, seedPoint2) = (seedPoint2, seedPoint3);
        }
        graphCenter = CalculateCircumCenter(seedPoint1, seedPoint2, seedPoint3);
        for (var pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            tempDistanceValues![pointIndex] = graphCenter.SqrDistance(points[pointIndex]);
        }
        // sort the points by distance from the seed triangle circumcenter
        QuicksortProcedure(0, points.Length - 1);
        // set up the seed triangle as the starting hull
        hullStart = seed1;
        hullSize = 3;
        tempHullNext![seed1] = tempHullPrev![seed3] = seed2;
        tempHullNext[seed2] = tempHullPrev[seed1] = seed3;
        tempHullNext[seed3] = tempHullPrev[seed2] = seed1;
        tempHullTri![seed1] = 0;
        tempHullTri[seed2] = 1;
        tempHullTri[seed3] = 2;
        tempHullHash![CalculateHashKey(seedPoint1)] = seed1;
        tempHullHash[CalculateHashKey(seedPoint2)] = seed2;
        tempHullHash[CalculateHashKey(seedPoint3)] = seed3;
        trianglePointsCount = 0;
        _ = AddTriangle(seed1, seed2, seed3, -1, -1, -1);
        Vec2<double> previousPoint = new(0);
        for (var k = 0; k < tempDistanceEdges!.Length; k++)
        {
            var currentPointIndex = tempDistanceEdges[k];
            var currentPoint = points[currentPointIndex];
            // skip near-duplicate points
            if (k > 0 && IsAproximate(currentPoint.x, previousPoint.x) && IsAproximate(currentPoint.y, previousPoint.y)) { continue; }
            previousPoint = currentPoint;
            // skip seed triangle points
            if (currentPointIndex == seed1 || currentPointIndex == seed2 || currentPointIndex == seed3) { continue; }
            // find a visible edge on the convex hull using edge hash
            var start = 0;
            var key = CalculateHashKey(currentPoint);
            for (var j = 0; j < hashSize; j++)
            {
                start = tempHullHash[(key + j) % hashSize];
                if (start != -1 && start != tempHullNext[start]) { break; }
            }
            start = tempHullPrev[start];
            var e = start;
            var q = tempHullNext[e];
            while (!NeedToReorient(currentPoint, points[e], points[q]))
            {
                e = q;
                if (e == start)
                {
                    e = int.MaxValue;
                    break;
                }

                q = tempHullNext[e];
            }
            if (e == int.MaxValue) { continue; }// likely a near-duplicate point; skip it
            // add the first triangle from the point
            var t = AddTriangle(e, currentPointIndex, tempHullNext[e], -1, -1, tempHullTri[e]);
            // recursively flip triangles from the point until they satisfy the Delaunay condition
            tempHullTri[currentPointIndex] = Legalize(t + 2);
            tempHullTri[e] = t; // keep track of boundary triangles on the hull
            hullSize++;
            // walk forward through the hull, adding more triangles and flipping recursively
            var next = tempHullNext[e];
            q = tempHullNext[next];
            while (NeedToReorient(currentPoint, points[next], points[q]))
            {
                t = AddTriangle(next, currentPointIndex, q, tempHullTri[currentPointIndex], -1, tempHullTri[next]);
                tempHullTri[currentPointIndex] = Legalize(t + 2);
                tempHullNext[next] = next; // mark as removed
                hullSize--;
                next = q;

                q = tempHullNext[next];
            }
            // walk backward from the other side, adding more triangles and flipping
            if (e == start)
            {
                q = tempHullPrev[e];

                while (NeedToReorient(currentPoint, points[q], points[e]))
                {
                    t = AddTriangle(q, currentPointIndex, e, -1, tempHullTri[e], tempHullTri[q]);
                    _ = Legalize(t + 2);
                    tempHullTri[q] = t;
                    tempHullNext[e] = e; // mark as removed
                    hullSize--;
                    e = q;

                    q = tempHullPrev[e];
                }
            }
            // update the hull indices
            hullStart = tempHullPrev[currentPointIndex] = e;
            tempHullNext[e] = tempHullPrev[next] = currentPointIndex;
            tempHullNext[currentPointIndex] = next;
            // save the two new edges in the hash table
            tempHullHash[CalculateHashKey(currentPoint)] = currentPointIndex;
            tempHullHash[CalculateHashKey(points[e])] = e;
        }
        // save hull indices
        if (hull is null || hull.Length != hullSize)
        {
            hull = new int[hullSize];
        }
        for (int i = 0, j = hullStart; i < hullSize; i++)
        {
            hull[i] = j;
            j = tempHullNext[j];
        }
        // trim triangles and halfedges array
        if (triangles.Length > trianglePointsCount)
        {
            Array.Resize(ref triangles, trianglePointsCount);
        }
        if (halfedges.Length > trianglePointsCount)
        {
            Array.Resize(ref halfedges, trianglePointsCount);
        }
    }
    private void ResizeTemporaryArrays()
    {
        static void RecreateArray<T>(ref T[]? array, int requestedSize)
        {
            if (array is null || array.Length < requestedSize)
            {
                array = new T[requestedSize];
            }
            else
            {
                Array.Clear(array, 0, requestedSize);
            }
        }
        hashSize = Mathf.CeilToInt(Mathf.Sqrt(points!.Length));
        RecreateArray(ref tempEdgeStack, 512);
        RecreateArray(ref tempHullNext, points.Length);
        RecreateArray(ref tempHullPrev, points.Length);
        RecreateArray(ref tempHullTri, points.Length);
        RecreateArray(ref tempHullHash, hashSize);
        RecreateArray(ref tempDistanceEdges, points.Length);
        RecreateArray(ref tempDistanceValues, points.Length);
    }
    public void TrimExcess()
    {
        tempEdgeStack = null;
        tempHullNext = null;
        tempHullPrev = null;
        tempHullTri = null;
        tempHullHash = null;
        tempDistanceEdges = null;
        tempDistanceValues = null;
    }
    public void Relax()
    {
        // We cant use 'GetRelaxedPoints' method because they miss the cell.CenterId.
        foreach (var cell in GetVoronoiCellsBasedOnCentroids())
        {
            points![cell.Center.id] = cell.IsClosed ? CalculateCentroid(cell.BorderPoints.ToArray()) : cell.Center.point;
        }
        Recalculate();
    }
    private int Legalize(int a)
    {
        var i = 0;
        int ar;
        // recursion eliminated with a fixed-size stack
        while (true)
        {
            var b = halfedges![a];
            /* if the pair of triangles doesn't satisfy the Delaunay condition
             * (p1 is inside the circumcircle of [p0, pl, pr]), flip them,
             * then do the same check/flip recursively for the new pair of triangles
             *
             *           pl                    pl
             *          /||\                  /  \
             *       al/ || \bl            al/    \a
             *        /  ||  \              /      \
             *       /  a||b  \    flip    /___ar___\
             *     p0\   ||   /p1   =>   p0\---bl---/p1
             *        \  ||  /              \      /
             *       ar\ || /br             b\    /br
             *          \||/                  \  /
             *           pr                    pr
             */
            var a0 = a - a % 3;
            ar = a0 + (a + 2) % 3;

            if (b == -1)
            { // convex hull edge
                if (i == 0)
                {
                    break;
                }

                a = tempEdgeStack![--i];
                continue;
            }
            var b0 = b - b % 3;
            var al = a0 + (a + 1) % 3;
            var bl = b0 + (b + 2) % 3;
            var p0 = triangles![ar];
            var pr = triangles[a];
            var pl = triangles[al];
            var p1 = triangles[bl];
            // Illegal
            if (InCircle(points![p0], points[pr], points[pl], points[p1]))
            {
                triangles[a] = p1;
                triangles[b] = p0;
                var hbl = halfedges[bl];
                // edge swapped on the other side of the hull (rare); fix the halfedge reference
                if (hbl == -1)
                {
                    var e = hullStart;
                    do
                    {
                        if (tempHullTri![e] == bl)
                        {
                            tempHullTri[e] = a;
                            break;
                        }
                        e = tempHullPrev![e];
                    } while (e != hullStart);
                }
                LinkHalfEdges(a, hbl);
                LinkHalfEdges(b, halfedges[ar]);
                LinkHalfEdges(ar, bl);
                var br = b0 + (b + 1) % 3;
                // don't worry about hitting the cap: it can only happen on extremely degenerate input
                if (i < tempEdgeStack!.Length)
                {
                    tempEdgeStack[i++] = br;
                }
            }
            else
            {
                if (i == 0)
                {
                    break;
                }

                a = tempEdgeStack![--i];
            }
        }
        return ar;
    }
    private static bool InCircle(in Vec2<double> a, in Vec2<double> b, in Vec2<double> c, in Vec2<double> p)
    {
        var d = a - p;
        var e = b - p;
        var f = c - p;
        var ap = d.x * d.x + d.y * d.y;
        var bp = e.x * e.x + e.y * e.y;
        var cp = f.x * f.x + f.y * f.y;
        return d.x * (e.y * cp - bp * f.y) -
               d.y * (e.x * cp - bp * f.x) +
               ap * (e.x * f.y - e.y * f.x) < 0;
    }
    private int AddTriangle(int i0, int i1, int i2, int a, int b, int c)
    {
        triangles![trianglePointsCount] = i0;
        triangles[trianglePointsCount + 1] = i1;
        triangles[trianglePointsCount + 2] = i2;
        LinkHalfEdges(trianglePointsCount, a);
        LinkHalfEdges(trianglePointsCount + 1, b);
        LinkHalfEdges(trianglePointsCount + 2, c);
        trianglePointsCount += 3;
        return trianglePointsCount - 3;
    }
    private void LinkHalfEdges(int a, int b)
    {
        halfedges![a] = b;
        if (b != -1) { halfedges[b] = a; }
    }
    private int CalculateHashKey(Vec2<double> position)
    {
        // Calculate position from the center
        position -= graphCenter;
        // Calculate pseudo angle
        var p = position.x / (Math.Abs(position.x) + Math.Abs(position.y));
        p = (position.y > 0 ? 3 - p : 1 + p) / 4; // [0..1]
        // Deliver a pseudo distance + angle hash
        return (int)(Math.Floor(p * hashSize) % hashSize);
    }
    private void QuicksortProcedure(int left, int right)
    {
        if (right - left <= 20)
        {
            for (var i = left + 1; i <= right; i++)
            {
                var temp = tempDistanceEdges![i];
                var tempDist = tempDistanceValues![temp];
                var j = i - 1;
                while (j >= left && tempDistanceValues[tempDistanceEdges[j]] > tempDist)
                {
                    tempDistanceEdges[j + 1] = tempDistanceEdges[j--];
                }
                tempDistanceEdges[j + 1] = temp;
            }
        }
        else
        {
            var median = (left + right) >> 1;
            var i = left + 1;
            var j = right;
            (tempDistanceEdges![i], tempDistanceEdges[median]) = (tempDistanceEdges[median], tempDistanceEdges[i]);
            if (tempDistanceValues![tempDistanceEdges[left]] > tempDistanceValues[tempDistanceEdges[right]])
            {
                (tempDistanceEdges[right], tempDistanceEdges[left]) = (tempDistanceEdges[left], tempDistanceEdges[right]);
            }
            if (tempDistanceValues[tempDistanceEdges[i]] > tempDistanceValues[tempDistanceEdges[right]])
            {
                (tempDistanceEdges[right], tempDistanceEdges[i]) = (tempDistanceEdges[i], tempDistanceEdges[right]);
            }
            if (tempDistanceValues[tempDistanceEdges[left]] > tempDistanceValues[tempDistanceEdges[i]])
            {
                (tempDistanceEdges[i], tempDistanceEdges[left]) = (tempDistanceEdges[left], tempDistanceEdges[i]);
            }
            var temp = tempDistanceEdges[i];
            var tempDist = tempDistanceValues[temp];
            while (true)
            {
                do
                {
                    i++;
                }
                while (tempDistanceValues[tempDistanceEdges[i]] < tempDist);
                do
                {
                    j--;
                }
                while (tempDistanceValues[tempDistanceEdges[j]] > tempDist);
                if (j < i)
                {
                    break;
                }

                (tempDistanceEdges[j], tempDistanceEdges[i]) = (tempDistanceEdges[i], tempDistanceEdges[j]);
            }
            tempDistanceEdges[left + 1] = tempDistanceEdges[j];
            tempDistanceEdges[j] = temp;
            if (right - i + 1 >= j - left)
            {
                QuicksortProcedure(i, right);
                QuicksortProcedure(left, j - 1);
            }
            else
            {
                QuicksortProcedure(left, j - 1);
                QuicksortProcedure(i, right);
            }
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NeedToReorient(in Vec2<double> a, in Vec2<double> b, in Vec2<double> c) => (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y) < 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vec2<double> PreCalculateCircum(in Vec2<double> a, in Vec2<double> b, in Vec2<double> c)
    {
        var ba = b - a;
        var ca = c - a;
        var baba = ba.x * ba.x + ba.y * ba.y;
        var caca = ca.x * ca.x + ca.y * ca.y;
        var baca = 0.5d / (ba.x * ca.y - ba.y * ca.x);
        return new((ca.y * baba - ba.y * caca) * baca, (ba.x * caca - ca.x * baba) * baca);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double CalculateCircumRadius(in Vec2<double> a, in Vec2<double> b, in Vec2<double> c)
    {
        var d = PreCalculateCircum(a, b, c);
        return d.x * d.x + d.y * d.y;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<double> CalculateCircumCenter(in Vec2<double> a, in Vec2<double> b, in Vec2<double> c) => PreCalculateCircum(a, b, c) + a;
    public IEnumerable<DelaunayTriangle> GetTriangles() => from i in Enumerable.Range(0, TrianglesCount) select GetTriangle(i);
    public IEnumerable<VoronoiCell> GetVoronoiCells(Func<int, Vec2<double>> triangleVerticeSelector)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IdentifiedPoint BuildTuple(int index) => new(triangleVerticeSelector.Invoke(GetTriangleOfEdge(index)), triangles[index]);
        if (triangleVerticeSelector is null)
        {
            throw new ArgumentException("'triangleVerticeSelector' should have a valid/non null function.");
        }
        var mapPointToEdge = new Dictionary<int, int>(points!.Length);
        for (var edgeIndex = 0; edgeIndex < triangles!.Length; edgeIndex++)
        {
            var pointIndex = triangles[CalculateNextHalfedge(edgeIndex)];
            if (!mapPointToEdge.ContainsKey(pointIndex) || halfedges![edgeIndex] == -1)
            {
                mapPointToEdge[pointIndex] = edgeIndex;
            }
        }
        var edges = new List<int>(16);
        for (var pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            if (!mapPointToEdge.ContainsKey(pointIndex)) { continue; }
            {
                var startEdgeIndex = mapPointToEdge[pointIndex];
                var incomingEdgeIndex = startEdgeIndex;
                while (true)
                {
                    edges.Add(incomingEdgeIndex);
                    var outgoingEdgeIndex = CalculateNextHalfedge(incomingEdgeIndex);
                    incomingEdgeIndex = halfedges![outgoingEdgeIndex];
                    if (incomingEdgeIndex == -1 || incomingEdgeIndex == startEdgeIndex)
                    {
                        if (edges.Count > 0)
                        {
                            if (edges.Count == 1)
                            {
                                edges.Clear();
                                edges.AddRange(from i in Enumerable.Range(GetTriangleOfEdge(startEdgeIndex) * 3, 3)
                                               where triangles[i] != startEdgeIndex
                                               select i);
                            }
                            yield return new(new(points[pointIndex], pointIndex),
                                             from edge in edges select BuildTuple(edge),
                                             incomingEdgeIndex == startEdgeIndex);
                        }
                        break;
                    }
                }
                edges.Clear();
            }
        }
    }
    public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCircumcenters() => GetVoronoiCells(GetTriangleCircumcenter);
    public IEnumerable<VoronoiCell> GetVoronoiCellsBasedOnCentroids() => GetVoronoiCells(GetCentroid);
    public IEnumerable<Vec2<double>> GetHullPoints()
    {
        return from i in hull
               select points![i];
    }
    public DelaunayTriangle GetTriangle(int triangleIndex)
    {
        return new(GetPointsOfTriangle(triangleIndex),
                   triangleIndex,
                   GetTrianglesAdjacentToTriangle(triangleIndex));
    }
    public IEnumerable<Vec2<double>> GetRelaxedPoints()
    {
        return from cell in GetVoronoiCellsBasedOnCentroids()
               select cell.IsClosed ? CalculateCentroid(cell.BorderPoints.ToArray()) : cell.Center.point;
    }
    public Vec2<double> GetTriangleCircumcenter(int triangleIndex)
    {
        var triangle = GetTriangle(triangleIndex);
        return CalculateCircumCenter(triangle.A,
                                     triangle.B,
                                     triangle.C);
    }
    public Vec2<double> GetCentroid(int triangleIndex)
    {
        var triangle = GetTriangle(triangleIndex);
        return CalculateCentroid(triangle.A,
                                 triangle.B,
                                 triangle.C);
    }
    public static Vec2<double> CalculateCentroid(params Vec2<double>[] points)
    {
        double accumulatedArea = default;
        Vec2<double> centroid = default;
        for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
        {
            var temp = points[i].x * points[j].y - points[j].x * points[i].y;
            accumulatedArea += temp;
            centroid += (points[i] + points[j]) * temp;
        }
        return IsAproximate(accumulatedArea, 0) ? default : centroid / (accumulatedArea * 3);
    }
    /// <summary>
    /// Returns the half-edges that share a start point with the given half edge, in 
    /// order until restarting or hitting a empty edge (-1).
    /// </summary>
    public IEnumerable<int> GetEdgesAroundPoint(int startEdgeIndex)
    {
        var incomingEdgeIndex = startEdgeIndex;
        while (true)
        {
            yield return incomingEdgeIndex;
            var outgoingEdgeIndex = CalculateNextHalfedge(incomingEdgeIndex);
            incomingEdgeIndex = halfedges![outgoingEdgeIndex];
            if (incomingEdgeIndex == -1 || incomingEdgeIndex == startEdgeIndex) { break; }
        }
    }
    /// <summary>
    /// Returns the three point indices of a given triangle id.
    /// </summary>
    public Triangle2<double> GetPointsOfTriangle(int triangleIndex)
    {
        var tri = GetEdgesOfTriangle(triangleIndex);
        return new()
        {
            A = points![triangles![tri.edgeA]],
            B = points[triangles[tri.edgeB]],
            C = points[triangles[tri.edgeC]]
        };
    }
    /// <summary>
    /// Returns the triangle ids adjacent to the given triangle id.
    /// Will return up to three values.
    /// </summary>
    public AdjacentTriangles GetTrianglesAdjacentToTriangle(int triangleIndex)
    {
        var tri = GetEdgesOfTriangle(triangleIndex);
        return new(halfedges![tri.edgeA] < 0 ? null : GetTriangleOfEdge(halfedges[tri.edgeA]),
                   halfedges[tri.edgeB] < 0 ? null : GetTriangleOfEdge(halfedges[tri.edgeB]),
                   halfedges[tri.edgeC] < 0 ? null : GetTriangleOfEdge(halfedges[tri.edgeC]));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// t0:t1,t1:t2,t2:t0
    /// </summary>
    /// <param name="edgeIndex"></param>
    /// <returns></returns>
    public static int CalculateNextHalfedge(int edgeIndex) => (edgeIndex % 3 == 2) ? edgeIndex - 2 : edgeIndex + 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// t0:t2,t1:t0,t2:t1
    /// </summary>
    /// <param name="edgeIndex"></param>
    /// <returns></returns>
    public static int CalculatePreviousHalfedge(int edgeIndex) => (edgeIndex % 3 == 0) ? edgeIndex + 2 : edgeIndex - 1;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// Returns the three half-edges of a given triangle id.
    /// </summary>
    public static TriangleEdges GetEdgesOfTriangle(int triangleIndex) => new(3 * triangleIndex);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// Returns the triangle id of a given half-edge.
    /// </summary>
    public static int GetTriangleOfEdge(int edgeIndex) => edgeIndex / 3;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsAproximate(double a, double b) => Math.Abs(a - b) <= double.Epsilon;
}