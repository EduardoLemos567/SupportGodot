using System;
using System.Numerics;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static N SqrtSaturated<N>(this N n) where N : INumber<N> => N.CreateSaturating(double.Sqrt(double.CreateSaturating(n)));
    /// <summary>
    /// Inverse Lerp function. It returns a value between 0 and 1 that represents the position of value between a and b.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <param name="t"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F InverseLerpBetween<F>(this F t, F a, F b) where F : IFloatingPoint<F>
    {
        if (a == b) { return F.One; }
        return (t - a) / (b - a);
    }
    /// <summary>
    /// Lerp function. It returns a value between a and b that represents the position of value between a and b.
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <param name="t"></param>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F LerpBetween<F>(this F t, F a, F b) where F : IFloatingPoint<F>
    {
        if (a == b) { return b; }
        return a + (b - a) * t;
    }
}
