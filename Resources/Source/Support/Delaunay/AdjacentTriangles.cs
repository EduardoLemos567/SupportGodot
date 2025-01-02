namespace Support.Delaunay
{
    /// <summary>
    /// Hold the index for 3 possible adjacent triangle from a triangle.
    /// If a triangle is null, means its on the border.
    /// </summary>
    public readonly struct AdjacentTriangles
    {
        public readonly int? triangleA;
        public readonly int? triangleB;
        public readonly int? triangleC;
        public bool IsBorder => !triangleA.HasValue || !triangleB.HasValue || !triangleC.HasValue;
        public AdjacentTriangles(int? triangleA, int? triangleB, int? triangleC)
        {
            this.triangleA = triangleA;
            this.triangleB = triangleB;
            this.triangleC = triangleC;
        }
    }
}