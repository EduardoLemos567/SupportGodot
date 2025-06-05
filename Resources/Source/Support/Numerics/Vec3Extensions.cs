using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Support.Numerics;

public static class Vec3Extensions
{
    #region FLOAT_POINT_ONLY
    public static Vec3<N> Round<F, N>(in this Vec3<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Round(self.x)),
        N.CreateChecked(F.Round(self.y)),
        N.CreateChecked(F.Round(self.z)));
    public static Vec3<N> Ceil<F, N>(in this Vec3<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Ceiling(self.x)),
        N.CreateChecked(F.Ceiling(self.y)),
        N.CreateChecked(F.Ceiling(self.z)));
    public static Vec3<N> Floor<F, N>(in this Vec3<F> self) where F : IFloatingPoint<F> where N : INumber<N> => new(
        N.CreateChecked(F.Floor(self.x)),
        N.CreateChecked(F.Floor(self.y)),
        N.CreateChecked(F.Floor(self.z)));
    public static F Magnitude<F>(in this Vec3<F> self) where F : IFloatingPoint<F>
    {
        return F.CreateChecked(Math.Sqrt(double.CreateChecked(self.SqrMagnitude())));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static F Distance<F>(in this Vec3<F> self, in Vec3<F> target) where F : IFloatingPoint<F> => (target - self).Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<F> Normalized<F>(in this Vec3<F> self) where F : IFloatingPoint<F> => self / self.Magnitude();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void NormalizedAndMagnitude<F>(in this Vec3<F> self, out Vec3<F> normalized, out F magnitude) where F : IFloatingPoint<F>
    {
        magnitude = self.Magnitude();
        normalized = self / magnitude;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<T> CrossProduct<T>(in this Vec3<T> self, in Vec3<T> target) where T : IFloatingPoint<T> => new(
        self.y * target.z - self.z * target.y,
        self.z * target.x - self.x * target.z,
        self.x * target.y - self.y * target.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsApproximate<F>(in this Vec3<F> self, in Vec3<F> target, F? proximity = null) where F : struct, IFloatingPoint<F>
    {
        F prox = proximity ?? IVectorNumber<F>.PROXIMITY_DISTANCE;
        return self.SqrDistance(target) < prox * prox;
    }
    public static Vec3<F> MoveTowards<F>(in this Vec3<F> self, in Vec3<F> target, F delta) where F : IFloatingPoint<F>
    {
        var diff = target - self;
        var sqrDist = diff.SqrMagnitude();
        var minDist = IVectorNumber<F>.PROXIMITY_DISTANCE;
        if (sqrDist <= delta * delta || sqrDist < minDist * minDist)
        {
            return target;
        }
        var dist = F.CreateChecked(Math.Sqrt(double.CreateChecked(sqrDist)));
        return self + diff / dist * delta;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<F> DirectionTowards<F>(in this Vec3<F> self, in Vec3<F> target) where F : IFloatingPoint<F>
    {
        return (target - self).Normalized();
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<F> LerpTo<F>(in this Vec3<F> self, in Vec3<F> target, F t) where F : IFloatingPoint<F>
    {
        return new(
            t.LerpBetween(self.x, target.x),
            t.LerpBetween(self.y, target.y),
            t.LerpBetween(self.z, target.z));
    }
    #endregion FLOAT_POINT_ONLY
    // System vector
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> ToVec3<N>(in this Vector3 self) where N : INumber<N> => Vec3<N>.CreateFrom(self.X, self.Y, self.Z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<float> ToVec3(in this Vector3 self) => ToVec3<float>(self);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(in this Vec3<float> self) => new(self.x, self.y, self.z);
}
