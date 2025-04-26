namespace Support.Delaunay;

/// <summary>
/// Hold the index of the 3 edges from a triangle.
/// They are stored in group of 3.
/// </summary>
public readonly struct TriangleEdges
{
    public readonly int edgeA;
    public readonly int edgeB;
    public readonly int edgeC;
    public TriangleEdges(int edgeA)
    {
        this.edgeA = edgeA;
        edgeB = edgeA + 1;
        edgeC = edgeA + 2;
    }
}