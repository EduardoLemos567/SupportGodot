using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Numerics;

public static class Vec4Extensions
{
    #region FLOAT_POINT_ONLY
    public static Vec4<N> Round<F, N>(in this Vec4<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Round(self.x)),
        N.CreateChecked(F.Round(self.y)),
        N.CreateChecked(F.Round(self.z)),
        N.CreateChecked(F.Round(self.w)));
    public static Vec4<N> Ceil<F, N>(in this Vec4<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Ceiling(self.x)),
        N.CreateChecked(F.Ceiling(self.y)),
        N.CreateChecked(F.Ceiling(self.z)),
        N.CreateChecked(F.Ceiling(self.w)));
    public static Vec4<N> Floor<F, N>(in this Vec4<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Floor(self.x)),
        N.CreateChecked(F.Floor(self.y)),
        N.CreateChecked(F.Floor(self.z)),
        N.CreateChecked(F.Floor(self.w)));
    public static F Magnitude<F>(in this Vec4<F> self) where F : IFloatingPoint<F>
    {
        return F.CreateChecked(Math.Sqrt(double.CreateChecked(self.SqrMagnitude())));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Distance<F>(in this Vec4<F> self, in Vec4<F> target) where F : IFloatingPoint<F> => (target - self).Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<F> Normalized<F>(in this Vec4<F> self) where F : IFloatingPoint<F> => self / self.Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximate<F>(in this Vec4<F> self, in Vec4<F> target, F? proximity = null) where F : struct, IFloatingPoint<F>
    {
        if (!proximity.HasValue) { proximity = IVectorNumber<F>.PROXIMITY_DISTANCE; }
        return self.Distance(target) < proximity;
    }
    public static Vec4<F> MoveTowards<F>(in this Vec4<F> self, in Vec4<F> target, F delta) where F : IFloatingPoint<F>
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
    public static Vec4<F> DirectionTowards<F>(in this Vec4<F> self, in Vec4<F> target) where F : IFloatingPoint<F>
    {
        return (target - self).Normalized();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<F> LerpTo<F>(in this Vec4<F> self, in Vec4<F> target, F t) where F : IFloatingPoint<F>
    {
        return new(
            t.LerpBetween(self.x, target.x),
            t.LerpBetween(self.y, target.y),
            t.LerpBetween(self.z, target.z),
            t.LerpBetween(self.w, target.w));
    }
    #endregion FLOAT_POINT_ONLY
    // System vector
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> ToVec4<N>(in this Vector4 self) where N : INumber<N> => Vec4<N>.CreateFrom(self.X, self.Y, self.Z, self.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<float> ToVec4(in this Vector4 self) => ToVec4(self);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(in this Vec4<float> self) => new(self.x, self.y, self.z, self.w);
}
