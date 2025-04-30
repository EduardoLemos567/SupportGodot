using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Godot;
using Support.Diagnostics;
using Support.Numerics;

namespace Support
{
    public static class Toolbox
    {
        public const float PROXIMITY_DISTANCE = 0.001f;
        public const float SQR_PROXIMITY_DISTANCE = PROXIMITY_DISTANCE * PROXIMITY_DISTANCE;
        public const int NULL_INDEX = -1;
        public static readonly float SQR2 = float.Sqrt(2);
        public static readonly float SQR3 = float.Sqrt(3);

        /// <summary>
        /// Turn 1.2345 into 1.23 if places = 2, into 1.234 if places = 3
        /// </summary>
        /// <param name="value"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        public static float Truncate(float value, int places)
        {
            var factor = (float)Math.Pow(10, places);
            return (int)(value * factor) / factor;
        }
        /// <summary>
        /// Turn 123456 into 123000 if places = 3
        /// </summary>
        /// <param name="value"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        public static int IntegerTruncate(int value, int places)
        {
            var factor = (int)Math.Pow(10, places);
            return value / factor * factor;
        }
        /// <summary>
        /// Remap one value from [imin...imax] into [omin...omax]
        /// </summary>
        /// <param name="value">Value to be interpolated</param>
        /// <param name="imin">Minimum input expected</param>
        /// <param name="imax">Maximum input expected</param>
        /// <param name="omin">Minimum output expected</param>
        /// <param name="omax">Maximum output expected</param>
        /// <returns></returns>
        public static float Remap(in float value, in float imin, in float imax, in float omin, in float omax) => omin + (omax - omin) * ((value - imin) / (imax - imin));
        public static float RemapClamped(in float value, in float imin, in float imax, in float omin, in float omax) => omin + (omax - omin) * Mathf.Clamp((value - imin) / (imax - imin), 0, 1);
        public static void LimitMod(ref Vec2<float> vec, float unit)
        {
            vec.x %= unit;
            vec.y %= unit;
        }
        public static Vec2<float> GetScreenSize(float scale = 1) => DisplayServer.ScreenGetSize().ToVec2<float>() * scale;
        public static Vec2<float> GetPositionAsPercentOfScreen(Vec2<float> position) => position / GetScreenSize();
        public static Vec2<float> GetPercentOfScreenAsPosition(Vec2<float> percent) => GetScreenSize() * percent;
        public static Vec2<float> GetMousePositionAsPercentOfScreen() => GetPositionAsPercentOfScreen(DisplayServer.MouseGetPosition().ToVec2<float>());
        public static string? GetRelativePath(string basePath, string fullPath)
        {
            basePath = basePath.Replace('\\', '/');
            fullPath = fullPath.Replace('\\', '/');
            if (fullPath.StartsWith(basePath))
            {
                var size = basePath.Length + (basePath.EndsWith('/') ? 0 : 1);
                return fullPath[size..];
            }
            else
            {
                return null;
            }
        }
        public static bool IsRelativePath(string basePath, string fullPath)
        {
            basePath = basePath.Replace('\\', '/');
            fullPath = fullPath.Replace('\\', '/');
            return fullPath.StartsWith(basePath);
        }
        public static bool CheckArrayEquals<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
        {
            if (arr1.Length == arr2.Length)
            {
                for (var i = 0; i < arr1.Length; i++)
                {
                    if (!arr1[i].Equals(arr2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public static int EnumLength<T>() where T : Enum => Enum.GetNames(typeof(T)).Length;
        public static E[] ListEnumValues<E>() where E : Enum => (E[])Enum.GetValues(typeof(E));
        public static bool CheckIsSorted<T>(T[] array) where T : IComparable<T>
        {
            for (var i = 1; i < array.Length; i++)
            {
                if (array[i - 1].CompareTo(array[i]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static int[] CreateRangeArray(int size)
        {
            var array = new int[size];
            for (var i = 0; i < size; i++) { array[i] = i; }
            return array;
        }
        public static string FormatSecondsToTime(int seconds)
        {
            return seconds < 0
                ? $"- {-seconds / 60}:{(-seconds % 60).ToString().PadLeft(2, '0')}"
                : $"{seconds / 60}:{(seconds % 60).ToString().PadLeft(2, '0')}";
        }
        public static int FormatedTimeToSeconds(string formated)
        {
            var negative = false;
            if (formated.StartsWith("- "))
            {
                negative = true;
                formated = formated[2..];
            }
            var parts = formated.Split(':');
            return (negative ? -1 : 1) * (int.Parse(parts[0]) * 60 + int.Parse(parts[1]));
        }
        public static float SpringInterpolation(float a, float b, float t)
        {
            t = Mathf.Clamp(t, 0, 1);
            t = (Mathf.Sin(t * Mathf.Pi * (.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + 1.2f * (1f - t));
            return a + (b - a) * t;
        }
        public static float SinInterpolation(float bottomExtremities, float peakCenter, float t) => Remap(Mathf.Sin(Mathf.Pi * t), -1, 1, bottomExtremities, peakCenter);
        public static bool CheckEqualsAny<T>(T value, params T[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (value!.Equals(values[i])) { return true; }
            }
            return false;
        }
        public static void CheckAndReorder<T, E>(T[] array)
        where T : IReordeable<E>
        where E : Enum
        {
            static int CompareItems(T item0, T item1)
            {
                var result = Convert.ToInt32(item0.Order).CompareTo(Convert.ToInt32(item1.Order));
                Debug.Assert(result != 0, $"Item0.Order '{item0.Order}' and Item1.Order '{item1.Order}', shouldn't be equal.");
                return result;
            }
            //First check: length
            Debug.Assert(array.Length == Toolbox.EnumLength<E>(), $"List size '{array.Length}' doesnt match to enum array '{EnumLength<E>()}'");
            Array.Sort(array, CompareItems);
        }
        public static int CeilNearestPowerOf2(int value)
        {
            if (value < 2)
            {
                return 1;
            }
            for (var i = 1; i < 30; i++)
            {
                var powerOf2 = 1 << i;
                if (powerOf2 >= value) { return powerOf2; }
            }
            return int.MaxValue;
        }
        public static int FloorNearestPowerOf2(int value)
        {
            var powerOf2 = 1 << 30;
            if (value >= powerOf2)
            {
                return powerOf2;
            }
            for (var i = 29; i > 0; i--)
            {
                powerOf2 = 1 << i;
                if (powerOf2 <= value) { return powerOf2; }
            }
            return 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPowerOf2(int value) => value != 0 && (Math.Log(value) / Math.Log(2) % 1) == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Pow2(int exp) => exp == 0 ? 1 : exp > 0 ? 2 << (exp - 1) : throw new ArgumentException("Expoent cant be negative");
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEven(float number) => number % 2 == 0;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(IEnumerable<T1> t1, IEnumerable<T2> t2)
        {
            var t1t = t1.GetEnumerator();
            var t2t = t2.GetEnumerator();
            while (t1t.MoveNext() && t2t.MoveNext())
            {
                yield return (t1t.Current, t2t.Current);
            }
        }
        /// <summary>
        /// This function implements the real modulo (equal to other languages), different from C# ('%').
        /// The difference is how it handles negative numbers.
        /// Normal C#: (-21 % 4 = -1)       & (21 % -4 = 1)
        /// Modulo   : (Modulo(-21, 4) = 3) & (Modulo(21, -4) = 1)
        /// </summary>
        /// <param name="x">dividend</param>
        /// <param name="m">divisor</param>
        /// <returns>modulo</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static N PositiveModulo<N>(N x, N m) where N : INumber<N>
        {
            var r = x % m;
            return r < N.Zero ? r + m : r;
        }
        /// <summary>
        /// Convert a sequence of input numbers to cultural invariant ToString.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numbers"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConvertInvariant<T>(params T[] numbers) => string.Join(", ", from n in numbers select Convert.ToString(n, CultureInfo.InvariantCulture.NumberFormat));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GridToIndex(in Vec2<int> g, int width) => g.y * width + g.x;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2<int> IndexToGrid(int index, int width) => new(index % width, index / width);
    }
}
