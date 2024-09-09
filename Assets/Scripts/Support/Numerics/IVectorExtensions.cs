using System;
using System.Numerics;

namespace Support.Numerics;

public static class IVectorExtensions
{
    public static int SizeOf<N>(this IVectorNumber<N> vector) where N : INumber<N> => vector.Length * vector[0].SizeOf();
    public static Type TypeOf<N>(this IVectorNumber<N> vector) where N : INumber<N> => vector[0].GetType();
    public static bool IsInteger<N>(this IVectorNumber<N> vector) where N : INumber<N> => N.IsInteger(vector[0]);
    public static N[] ToArray<N>(this IVectorNumber<N> vector) where N : INumber<N>
    {
        var a = new N[vector.Length];
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = vector[i];
        }
        return a;
    }
    public static int SizeOf(this IVectorBool vector) => vector.Length;
    public static Type TypeOf(this IVectorBool _) => typeof(bool);
    public static bool[] ToArray(this IVectorBool vector)
    {
        var a = new bool[vector.Length];
        for (int i = 0; i < a.Length; i++)
        {
            a[i] = vector[i];
        }
        return a;
    }
}
