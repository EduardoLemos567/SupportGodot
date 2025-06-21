using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Support.Geometrics;
using Support.Numerics;

namespace Support.Rng;

/// <summary>
/// Allow different implementations of RNGs.
/// </summary>
public abstract partial class ARng
{
    private const int ALPHABET_COUNT = 26;
    private static readonly Range<int> charNumberRange = new(48, 57);
    private static readonly Range<int> charLowerRange = new(97, 122);
    private static readonly Range<int> charUpperRange = new(65, 90);
    /// <summary>
    /// Time based seed, counted since OS startup.
    /// </summary>    
    public static uint TimeSeed => (uint)Environment.TickCount;
    public uint Seed { get; protected set; }
    protected ARng(object? seed = null) => Reseed(seed);
    public abstract T GetNumber<T>(T min, T max) where T : INumber<T>;
    public abstract void Reset();
    public void Reseed(object? seed)
    {
        Seed = seed is null ? 0 : (uint)seed.GetHashCode();
        if (Seed == 0) { Seed = TimeSeed; }
        Reset();
    }
    public bool GetTrueByChance<F>(F chance) where F : IFloatingPoint<F>
    {
        return chance > F.Zero && (chance >= F.One || GetNumber(F.Zero, F.One) <= chance);
    }
    public N GetPointIn<N>(in Range<N> range) where N : INumber<N> => GetNumber(range.Min, range.Max);
    public Vec2<N> GetPointIn<N>(in Rectangle<N> rect) where N : INumber<N> => GetVec2(rect.Min, rect.Max);
    public Vec2<N> GetPointIn<N>(in Circle<N> circle) where N : INumber<N>
    {
        var randomCircle = new Circle<N>() { center = circle.center, Radius = GetNumber(N.Zero, circle.Radius) };
        return randomCircle.GetPointInCircumference(RadianAngle.RandomAngle(this));
    }
    public string GetString(int count, bool includeLower = true, bool includeUpper = true) => string.Create<(ARng rng, bool lower, bool upper)>(count, (this, includeLower, includeUpper), CreateString);
    private static void CreateString(Span<char> result, (ARng rng, bool lower, bool upper) state)
    {
        var range = 10 + (state.lower ? ALPHABET_COUNT : 0) + (state.upper ? ALPHABET_COUNT : 0);
        for (var i = 0; i < result.Length; i++)
        {
            var value = state.rng.GetNumber(0, range);
            byte direction = 0;
            if (value >= 10)
            {
                if (value < (10 + ALPHABET_COUNT))
                {
                    value -= 10;
                    direction = !state.lower ? (byte)2 : (byte)1;
                }
                else
                {
                    value -= 10 + ALPHABET_COUNT;
                    direction = 2;
                }
            }
            switch (direction)
            {
                case 0:
                    result[i] = (char)(charNumberRange.Min + value);
                    break;
                case 1:
                    result[i] = (char)(charLowerRange.Min + value);
                    break;
                case 2:
                    result[i] = (char)(charUpperRange.Min + value);
                    break;
            }
        }
    }
    public Color GetColor() => new() { r = GetNumber<float>(0, 1), g = GetNumber<float>(0, 1), b = GetNumber<float>(0, 1), a = 1 };
    public Vec2<N> GetVec2<N>(N minValue, N maxValue) where N : INumber<N> => new(GetNumber(minValue, maxValue), GetNumber(minValue, maxValue));
    public Vec2<N> GetVec2<N>(in Vec2<N> minValue, in Vec2<N> maxValue) where N : INumber<N> => new(GetNumber(minValue.x, maxValue.x), GetNumber(minValue.y, maxValue.y));
    public Vec3<N> GetVec3<N>(N minValue, N maxValue) where N : INumber<N> => new(GetNumber(minValue, maxValue), GetNumber(minValue, maxValue), GetNumber(minValue, maxValue));
    public Vec3<N> GetVec3<N>(in Vec3<N> minValue, in Vec3<N> maxValue) where N : INumber<N> => new(GetNumber(minValue.x, maxValue.x), GetNumber(minValue.y, maxValue.y), GetNumber(minValue.z, maxValue.z));
    public Vec4<N> GetVec4<N>(N minValue, N maxValue) where N : INumber<N> => new(GetNumber(minValue, maxValue), GetNumber(minValue, maxValue), GetNumber(minValue, maxValue), GetNumber(minValue, maxValue));
    public Vec4<N> GetVec4<N>(in Vec4<N> minValue, in Vec4<N> maxValue) where N : INumber<N> => new(GetNumber(minValue.x, maxValue.x), GetNumber(minValue.y, maxValue.y), GetNumber(minValue.z, maxValue.z), GetNumber(minValue.w, maxValue.w));
    public Direction GetDirection() => Direction.FromEnum(Pick<Direction.ENUM>(false));
    public void ShuffleList<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = GetNumber(0, n);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    public E? Pick<E>(bool excludeZero) where E : Enum
    {
        var values = Enum.GetValues(typeof(E));
        return values.Length > 0 ? (E?)values.GetValue(GetNumber(excludeZero ? 1 : 0, values.Length)) : (E?)values.GetValue(0);
    }
    public T Pick<T>(T[] array) => array.Length > 0 ? array[GetNumber(0, array.Length)] : throw new ArgumentException("Empty array");
    public T Pick<T>(IList<T> list) => list.Count > 0 ? list[GetNumber(0, list.Count)] : throw new ArgumentException("Empty list");
    public T Pick<T>(ICollection<T> collection) => collection.Count > 0 ? collection.Skip(GetNumber(0, collection.Count)).First() : throw new ArgumentException("Empty collection");
    public void Sample<T>(IReadOnlyList<T> list, int total, IList<T> result)
    {
        var indices = new int[list.Count];
        ShuffleList(indices);
        for (var i = 0; i < total; i++)
        {
            result.Add(list[indices[i]]);
        }
    }
}