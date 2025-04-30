using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Numerics;

public static class Vec2Extensions
{
    #region FLOAT_POINT_ONLY
    public static Vec2<N> Round<F, N>(in this Vec2<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Round(self.x)),
        N.CreateChecked(F.Round(self.y)));
    public static Vec2<N> Ceil<F, N>(in this Vec2<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Ceiling(self.x)),
        N.CreateChecked(F.Ceiling(self.y)));
    public static Vec2<N> Floor<F, N>(in this Vec2<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Floor(self.x)),
        N.CreateChecked(F.Floor(self.y)));
    public static F Magnitude<F>(in this Vec2<F> self) where F : IFloatingPoint<F>
    {
        return F.CreateChecked(Math.Sqrt(double.CreateChecked(self.SqrMagnitude())));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Distance<F>(in this Vec2<F> self, in Vec2<F> target) where F : IFloatingPoint<F> => (target - self).Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<F> Normalized<F>(in this Vec2<F> self) where F : IFloatingPoint<F> => self / self.Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximate<F>(in this Vec2<F> self, in Vec2<F> target, F? proximity = null) where F : struct, IFloatingPoint<F>
    {
        if (!proximity.HasValue) { proximity = IVectorNumber<F>.PROXIMITY_DISTANCE; }
        return self.Distance(target) < proximity;
    }
    public static Vec2<F> MoveTowards<F>(in this Vec2<F> self, in Vec2<F> target, F delta) where F : IFloatingPoint<F>
    {
        var diff = target - self;
        var dist = diff.Magnitude();
        if (dist <= delta || dist < IVectorNumber<F>.PROXIMITY_DISTANCE)
        {
            return target;
        }
        return self + diff / dist * delta;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<F> DirectionTowards<F>(in this Vec2<F> self, in Vec2<F> target) where F : IFloatingPoint<F>
    {
        return (target - self).Normalized();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<F> LerpTo<F>(in this Vec2<F> self, in Vec2<F> target, F t) where F : IFloatingPoint<F>
    {
        return new(
            t.LerpBetween(self.x, target.x),
            t.LerpBetween(self.y, target.y));
    }
    #endregion FLOAT_POINT_ONLY
    public static IEnumerable<Vec2<int>> EnumeratePositions(this Vec2<int> vec)
    {
        for (int x = 0; x < vec.x; x++)
        {
            for (int y = 0; y < vec.y; y++)
            {
                yield return new(x, y);
            }
        }
    }
    // System vector
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> ToVec2<N>(in this Vector2 self) where N : INumber<N> => Vec2<N>.CreateFrom(self.X, self.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<float> ToVec2(in this Vector2 self) => ToVec2(self);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(in this Vec2<float> self) => new(self.x, self.y);
}
