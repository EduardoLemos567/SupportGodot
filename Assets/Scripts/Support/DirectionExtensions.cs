using Support.Diagnostics;

namespace Support;

public static class DirectionExtensions
{
    public static Direction RadialSearch(this in BitVector8 bits, in Direction startingDirection, bool clockWise)
    {
        Debug.Assert(!startingDirection.IsNone, "'startingPoint' Direction cant be None.");
        foreach (var direction in
            clockWise ?
            Direction.LoopClockwise(startingDirection)
            : Direction.LoopCounterClockwise(startingDirection)
        )
        {
            if (bits[direction.AsIndex]) { return direction; }
        }
        return startingDirection;
    }
}
