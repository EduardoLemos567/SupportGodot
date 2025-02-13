using Support.Numerics;

namespace Support.Geometrics.Hexagons.Pointy;

/// <summary>
/// Direction of neighbor hexagons.
/// </summary>
public enum E_TRANSLATION : byte
{
    RIGHT,
    DOWN_RIGHT,
    DOWN_LEFT,
    LEFT,
    UP_LEFT,
    UP_RIGHT,
}

public struct Coordinate
{
    public Vec2<int> offset;
    public Coordinate(in Vec2<int> offset) => this.offset = offset;
    /// <summary>
    /// Get the neighbor offset based on internal offset and E_TRANSLATION direction.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public readonly Vec2<int> GetNeighborOffset(in E_TRANSLATION direction)
    {
        return direction switch
        {
            E_TRANSLATION.UP_RIGHT => (Toolbox.IsEven(offset.y) ? new Vec2<int>(0, -1) : new Vec2<int>(1, -1)),
            E_TRANSLATION.RIGHT => new Vec2<int>(1, 0),
            E_TRANSLATION.DOWN_RIGHT => (Toolbox.IsEven(offset.y) ? new Vec2<int>(0, 1) : new Vec2<int>(1, 1)),
            E_TRANSLATION.DOWN_LEFT => (Toolbox.IsEven(offset.y) ? new Vec2<int>(-1, 1) : new Vec2<int>(0, 1)),
            E_TRANSLATION.LEFT => new Vec2<int>(-1, 0),
            _ => (Toolbox.IsEven(offset.y) ? new Vec2<int>(-1, -1) : new Vec2<int>(0, -1))
        };
    }
    /// <summary>
    /// Modify the offset towards a direction by just one step.
    /// </summary>
    /// <param name="direction"></param>
    public void Translate(in E_TRANSLATION direction) => offset += GetNeighborOffset(direction);
    /// <summary>
    /// Modify the offset towards a direction by certain amount of steps.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="steps"></param>
    public void TranslateSteps(in E_TRANSLATION direction, int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            Translate(direction);
        }
    }
    public override readonly int GetHashCode() => offset.GetHashCode();
    public override readonly string ToString() => $"Coordinate({offset})";
}
