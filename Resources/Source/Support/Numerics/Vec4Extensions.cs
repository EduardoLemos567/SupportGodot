using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Numerics;

public static class Vec4Extensions
{
    #region FLOAT_POINT_ONLY
    public static Vec4<T> Round<F, T>(in this Vec4<F> self) where F : IFloatingPoint<F> where T : INumber<T> => new(
        T.CreateChecked(F.Round(self.x)),
        T.CreateChecked(F.Round(self.y)),
        T.CreateChecked(F.Round(self.z)),
        T.CreateChecked(F.Round(self.w)));
    public static Vec4<T> Ceil<F, T>(in this Vec4<F> self) where F : IFloatingPoint<F> where T : INumber<T> => new(
        T.CreateChecked(F.Ceiling(self.x)),
        T.CreateChecked(F.Ceiling(self.y)),
        T.CreateChecked(F.Ceiling(self.z)),
        T.CreateChecked(F.Ceiling(self.w)));
    public static Vec4<T> Floor<F, T>(in this Vec4<F> self) where F : IFloatingPoint<F> where T : INumber<T> => new(
        T.CreateChecked(F.Floor(self.x)),
        T.CreateChecked(F.Floor(self.y)),
        T.CreateChecked(F.Floor(self.z)),
        T.CreateChecked(F.Floor(self.w)));
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
    public static Vec4<float> Lerp(in this Vec4<float> self, in Vec4<float> target, float t)
    {
        return new(
            float.Lerp(self.x, target.x, t),
            float.Lerp(self.y, target.y, t),
            float.Lerp(self.z, target.z, t),
            float.Lerp(self.w, target.w, t));
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
