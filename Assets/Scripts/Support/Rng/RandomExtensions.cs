using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Support.Geometrics;
using Support.Numerics;

namespace Support.Rng;

/// <summary>
/// Implement complementary functions to be used in certain circumstances.
/// Extension functions works also on structs if the IRng is struct.
/// </summary>
public static class RandomExtensions
{
    private const int ALPHABET_COUNT = 26;
    private static readonly Range<int> charNumberRange = new(48, 57);
    private static readonly Range<int> charLowerRange = new(97, 122);
    private static readonly Range<int> charUpperRange = new(65, 90);
    public static bool GetTrueByChance<T>(this IRng rng, T chance) where T : IFloatingPoint<T>
    {
        var zero = T.CreateChecked(0);
        var one = T.CreateChecked(1);
        return chance > zero && (chance >= one || rng.GetNumber(zero, one) <= chance);
    }

    public static T GetPointIn<T>(this IRng rng, in Range<T> range) where T : INumber<T> => rng.GetNumber(range.Min, range.Max);
    public static Vec2<T> GetPointIn<T>(this IRng rng, in Rectangle<T> rect) where T : INumber<T> => rng.GetVec2(rect.min, rect.Max);
    public static Vec2<T> GetPointIn<T>(this IRng rng, in Circle<T> circle) where T : INumber<T>
    {
        var randomCircle = new Circle<T>() { center = circle.center, Radius = rng.GetNumber(T.CreateChecked(0), circle.Radius) };
        return randomCircle.GetPointInCircumference(RadianAngle.RandomAngle(rng));
    }
    public static string GetString(this IRng rng, int count, bool includeLower = true, bool includeUpper = true) => string.Create<(IRng rng, bool lower, bool upper)>(count, (rng, includeLower, includeUpper), CreateString);
    private static void CreateString(Span<char> result, (IRng rng, bool lower, bool upper) state)
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
    public static Color GetColor(this IRng rng) => new() { r = rng.GetNumber<float>(0, 1), g = rng.GetNumber<float>(0, 1), b = rng.GetNumber<float>(0, 1), a = 1 };
    public static Vec2<T> GetVec2<T>(this IRng rng, T minValue, T maxValue) where T : INumber<T> => new(rng.GetNumber(minValue, maxValue), rng.GetNumber(minValue, maxValue));
    public static Vec2<T> GetVec2<T>(this IRng rng, Vec2<T> minValue, Vec2<T> maxValue) where T : INumber<T> => new(rng.GetNumber(minValue.x, maxValue.x), rng.GetNumber(minValue.y, maxValue.y));
    public static Vec3<T> GetVec3<T>(this IRng rng, T minValue, T maxValue) where T : INumber<T> => new(rng.GetNumber(minValue, maxValue), rng.GetNumber(minValue, maxValue), rng.GetNumber(minValue, maxValue));
    public static Vec3<T> GetVec3<T>(this IRng rng, Vec3<T> minValue, Vec3<T> maxValue) where T : INumber<T> => new(rng.GetNumber(minValue.x, maxValue.x), rng.GetNumber(minValue.y, maxValue.y), rng.GetNumber(minValue.z, maxValue.z));
    public static Vec4<T> GetVec4<T>(this IRng rng, T minValue, T maxValue) where T : INumber<T> => new(rng.GetNumber(minValue, maxValue), rng.GetNumber(minValue, maxValue), rng.GetNumber(minValue, maxValue));
    public static Vec4<T> GetVec4<T>(this IRng rng, Vec4<T> minValue, Vec4<T> maxValue) where T : INumber<T> => new(rng.GetNumber(minValue.x, maxValue.x), rng.GetNumber(minValue.y, maxValue.y), rng.GetNumber(minValue.z, maxValue.z), rng.GetNumber(minValue.w, maxValue.w));
    public static Direction GetDirection(this IRng rng) => new(Pick<Direction.DIRECTIONS>(rng, false));
    public static void ShuffleList<T>(this IRng rng, IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = rng.GetNumber(0, n);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
    public static T? Pick<T>(this IRng rng, bool excludeZero) where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return values.Length > 0 ? (T?)values.GetValue(rng.GetNumber(excludeZero ? 1 : 0, values.Length)) : (T?)values.GetValue(0);
    }
    public static T Pick<T>(this IRng rng, T[] array) => array.Length > 0 ? array[rng.GetNumber(0, array.Length)] : throw new ArgumentException("Empty array");
    public static T Pick<T>(this IRng rng, IList<T> list) => list.Count > 0 ? list[rng.GetNumber(0, list.Count)] : throw new ArgumentException("Empty list");
    public static T Pick<T>(this IRng rng, ICollection<T> collection) => collection.Count > 0 ? collection.Skip(rng.GetNumber(0, collection.Count)).First() : throw new ArgumentException("Empty collection");
    public static void Sample<T>(this IRng rng, IReadOnlyList<T> list, int total, IList<T> result)
    {
        var indices = new int[list.Count];
        ShuffleList(rng, indices);
        for (var i = 0; i < total; i++)
        {
            result.Add(list[indices[i]]);
        }
    }
}