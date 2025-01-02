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
    public static bool IsInteger<T>(this T value) where T : INumber<T>
    {
        return T.IsInteger(value);
    }
}
