using System;
using System.Numerics;

namespace Support.Numerics;

public static class NumericsExtensions
{
    public static int SizeOf<T>(this T value) where T : INumber<T>
    {
        return value switch
        {
            byte => 1,
            sbyte => 1,
            ushort => 2,
            short => 2,
            uint => 4,
            int => 4,
            ulong => 8,
            long => 8,
            float => 4,
            double => 8,
            decimal => 16,
            _ => throw new NotSupportedException("Type not supported"),
        };
    }
    public static bool IsInteger<T>(this T value) where T : INumber<T> => T.IsInteger(value);
    public static bool IsApproximate<F>(this F self, in F target, F? proximity = null) where F : struct, IFloatingPoint<F>
    {
        if (self == target) { return true; }
        if (!proximity.HasValue) { proximity = F.CreateChecked(Toolbox.PROXIMITY_DISTANCE); }
        return F.Abs(target - self) < proximity;
    }
    /// <summary>
    /// Apply the <see cref="double.Sqrt"/> and saturate (clamp) the result in the range of MinValue and MaxValue of the type.
    /// </summary>
    /// <typeparam name="N"></typeparam>
    /// <param name="n"></param>
    /// <returns></returns>
    public static N SqrtSaturated<N>(this N n) where N : INumber<N> => N.CreateSaturating(double.Sqrt(double.CreateSaturating(n)));
}
