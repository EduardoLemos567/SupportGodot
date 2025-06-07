using System.Numerics;
using System.Runtime.CompilerServices;

using GodotVector2 = Godot.Vector2;
using GodotVector2I = Godot.Vector2I;
using GodotVector3 = Godot.Vector3;
using GodotVector3I = Godot.Vector3I;
using GodotVector4 = Godot.Vector4;
using GodotVector4I = Godot.Vector4I;

namespace Support.Numerics;

public static class GodotVectorExtensions
{
    // GodotVector2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> ToVec2<N>(in this GodotVector2 from) where N : INumber<N> => Vec2<N>.CreateFrom(from.X, from.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<float> ToVec2(in this GodotVector2 from) => ToVec2<float>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector2 ToGodotVector2(in this Vec2<float> from) => new(from.x, from.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector3 ToGodotVector3XZ<N>(in this Vec2<N> from, float y = 0) where N : INumber<N>
    {
        return new(float.CreateChecked(from.x), y, float.CreateChecked(from.y));
    }
    // GodotVector2I
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<N> ToVec2<N>(in this GodotVector2I from) where N : INumber<N> => Vec2<N>.CreateFrom(from.X, from.Y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec2<int> ToVec2(in this GodotVector2I from) => ToVec2<int>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector2I ToGodotVector2I(in this Vec2<int> from) => new(from.x, from.y);
    // GodotVector3
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> ToVec3<N>(in this GodotVector3 from) where N : INumber<N> => Vec3<N>.CreateFrom(from.X, from.Y, from.Z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<float> ToVec3(in this GodotVector3 from) => ToVec3<float>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector3 ToGodotVector3(in this Vec3<float> from) => new(from.x, from.y, from.z);
    // GodotVector3I
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<N> ToVec3<N>(in this GodotVector3I from) where N : INumber<N> => Vec3<N>.CreateFrom(from.X, from.Y, from.Z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3<int> ToVec3(in this GodotVector3I from) => ToVec3<int>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector3I ToGodotVector3I(in this Vec3<int> from) => new(from.x, from.y, from.z);
    // GodotVector4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> ToVec4<N>(in this GodotVector4 from) where N : INumber<N> => Vec4<N>.CreateFrom(from.X, from.Y, from.Z, from.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<float> ToVec4(in this GodotVector4 from) => ToVec4<float>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector4 ToGodotVector4(in this Vec4<float> from) => new(from.x, from.y, from.z, from.w);
    // GodotVector4I
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<N> ToVec4<N>(in this GodotVector4I from) where N : INumber<N> => Vec4<N>.CreateFrom(from.X, from.Y, from.Z, from.W);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec4<int> ToVec4(in this GodotVector4I from) => ToVec4<int>(from);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GodotVector4I ToGodotVector4I(in this Vec4<int> from) => new(from.x, from.y, from.z, from.w);
}