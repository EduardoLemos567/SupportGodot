﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Support.Extensions;

public static class ListExtensions
{
    /// <summary>
    /// Takes a extra value and increase capacity enough to accomodate that extra.
    /// Considers current count. Ignores if capacity is enough.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="etra"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureExtraCapacity<T>(this List<T> list, int etra)
    {
        if (list.Capacity >= list.Count + etra)
        {
            return;
        }
        list.Capacity = list.Count + etra;
    }
    /// <summary>
    /// Returns and remove a range from a List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Pop<T>(this List<T> list, in Range range)
    {
        (var offset, var count) = range.GetOffsetAndLength(list.Count);
        try { return list.GetRange(offset, count); }
        finally { list.RemoveRange(offset, count); }
    }
    #region ILIST
    /// <summary>
    /// Returns and remove a value from a IList.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Pop<T>(this IList<T> list, in Index index)
    {
        try { return list[index]; }
        finally { list.RemoveAt(index.GetOffset(list.Count)); }
    }
    #endregion ILIST
    #region IREADONLYLIST
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LoopIndex<T>(this IReadOnlyList<T> ilist, int index) => Toolbox.Modulo(index, ilist.Count);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetOnLoopIndex<T>(this IReadOnlyList<T> ilist, int index) => ilist[Toolbox.Modulo(index, ilist.Count)];
    #endregion IREADONLYLIST
}