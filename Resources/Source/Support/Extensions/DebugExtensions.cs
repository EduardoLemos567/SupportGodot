using System;
using System.Collections.Generic;

namespace Support.Extensions;

public static class DebugExtensions
{
    public static string PrintContent<T>(this IEnumerable<T> list)
    {
        var items = new List<string>();
        foreach (var item in list)
        {
            items.Add(item!.ToString() ?? "null");
        }
        return $"[{string.Join(",\n", items)}]";
    }
    public static string PrintContent<K, V>(this IEnumerable<KeyValuePair<K, V>> dict)
    {
        var items = new List<string>();
        foreach (var item in dict)
        {
            items.Add($"{item.Key}:{item.Value}");
        }
        return $"{{{string.Join(",\n", items)}}}";
    }
    public static string PrintContent<T>(this ReadOnlySpan<T> rospan)
    {
        var items = new List<string>();
        foreach (var item in rospan)
        {
            items.Add(item!.ToString() ?? "null");
        }
        return $"[{string.Join(",\n", items)}]";
    }
}