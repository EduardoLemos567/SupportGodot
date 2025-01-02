using System;
using System.Collections.Generic;
using Support.Extensions;

namespace Support.Cache;

public partial class ArrayCache<T>
{
    public static ArrayCache<T> Shared => _shared ??= new();
    public static bool HasShared => _shared is not null;
    public uint cacheLimit; // 0 = unlimited
    private uint cacheInUse;
    private static ArrayCache<T>? _shared;
    private readonly SortedDictionary<int, List<(T[] array, DateTime time)>> cacheSortedDictionary = new();
    public virtual T[] Rent(int minSize)
    {
        minSize = Toolbox.CeilNearestPowerOf2(minSize);
        lock (this)
        {
            foreach (var item in cacheSortedDictionary)
            {
                if (item.Key > minSize)
                {
                    if (cacheSortedDictionary[item.Key].Count > 0)
                    {
                        cacheInUse--;
                        return cacheSortedDictionary[item.Key].Pop(^1).array;
                    }
                    break;
                }
            }
        }
        return new T[minSize];
    }
    public Returnable RentReturnable(int minSize) => new(this, Rent(minSize));
    public ArraySegment<T> RentSegment(int minSize) => new(Rent(minSize), 0, minSize);
    public ReturnableSegment RentReturnableSegment(int minSize) => new(this, RentSegment(minSize));
    public virtual void Return(T[] array)
    {
        if (!Toolbox.IsPowerOf2(array.Length))
        {
            throw new ArgumentException("Array.Length must be a power of two.");
        }
        lock (this)
        {
            if (cacheLimit > 0 && cacheInUse >= cacheLimit) { return; }
            Array.Clear(array, 0, array.Length);
            if (!cacheSortedDictionary.ContainsKey(array.Length))
            {
                cacheSortedDictionary.Add(array.Length, new());
            }
            cacheSortedDictionary[array.Length].Add((array, DateTime.Now));
            cacheInUse++;
        }
    }
    public void ClearAndTrim()
    {
        lock (this)
        {
            cacheSortedDictionary.Clear();
            cacheInUse = 0;
        }
    }
    public void TrimOlder(DateTime older)
    {
        lock (this)
        {
            foreach (var pair in cacheSortedDictionary)
            {
                var shouldTrim = false;
                for (var i = 0; i < pair.Value.Count; i++)
                {
                    if (pair.Value[i].time < older)
                    {
                        pair.Value.RemoveAt(i);
                        i--;
                        cacheInUse--;
                        shouldTrim = true;
                    }
                }
                if (pair.Value.Count == 0) { _ = cacheSortedDictionary.Remove(pair.Key); }
                else if (shouldTrim) { pair.Value.TrimExcess(); }
            }
        }
    }
}