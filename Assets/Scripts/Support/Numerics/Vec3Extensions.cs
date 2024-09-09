using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Numerics;

public static class Vec3Extensions
{
    #region FLOAT_POINT_ONLY
    public static Vec3<F> Round<F>(in this Vec3<F> self) where F : IFloatingPoint<F> => new(
        F.Round(self.x),
        F.Round(self.y),
        F.Round(self.z));
    public static Vec3<F> Ceil<F>(in this Vec3<F> self) where F : IFloatingPoint<F> => new(
        F.Ceiling(self.x),
        F.Ceiling(self.y),
        F.Ceiling(self.z));
    public static Vec3<F> Floor<F>(in this Vec3<F> self) where F : IFloatingPoint<F> => new(
        F.Floor(self.x),
        F.Floor(self.y),
        F.Floor(self.z));
    public static F Magnitude<F>(in this Vec3<F> self) where F : IFloatingPoint<F>
    {
        return F.CreateChecked(Math.Sqrt(double.CreateChecked(self.SqrMagnitude())));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Distance<F>(in this Vec3<F> self, in Vec3<F> target) where F : IFloatingPoint<F> => (target - self).Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<F> Normalized<F>(in this Vec3<F> self) where F : IFloatingPoint<F> => self / self.Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<T> CrossProduct<T>(in this Vec3<T> self, in Vec3<T> target) where T : IFloatingPoint<T> => new(
        self.y * target.z - self.z * target.y,
        self.z * target.x - self.x * target.z,
        self.x * target.y - self.y * target.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximate<F>(in this Vec3<F> self, in Vec3<F> target, F? proximity = null) where F : struct, IFloatingPoint<F>
    {
        if (!proximity.HasValue) { proximity = IVectorNumber<F>.PROXIMITY_DISTANCE; }
        return self.Distance(target) < proximity;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<F> MoveTowards<F>(in this Vec3<F> self, in Vec3<F> target, F delta) where F : IFloatingPoint<F>
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
    public static Vec3<F> DirectionTowards<F>(in this Vec3<F> self, in Vec3<F> target) where F : IFloatingPoint<F>
    {
        return (target - self).Normalized();
    }
    #endregion FLOAT_POINT_ONLY
    // System vector
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> ToVec3<N>(in this Vector3 self) where N : INumber<N> => Vec3<N>.CreateFrom(self.X, self.Y, self.Z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<float> ToVec3(in this Vector3 self) => ToVec3(self);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(in this Vec3<float> self) => new(self.x, self.y, self.z);
}
