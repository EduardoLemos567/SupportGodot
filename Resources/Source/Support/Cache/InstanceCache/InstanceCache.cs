using System;
using System.Collections.Generic;
using Support.Extensions;

namespace Support.Cache;

public partial class InstanceCache<T> where T : class, new()
{
    public static InstanceCache<T> Shared => _shared ??= new();
    private static InstanceCache<T>? _shared;
    public static bool HasShared => _shared is not null;
    public uint cacheLimit; // 0 = unlimited
    private readonly List<(T instance, DateTime time)> cacheList = new();
    public Returnable RentReturnable() => new(this, Rent());
    public virtual T Rent()
    {
        T instance;
        lock (this)
        {
            if (cacheList.Count == 0)
            {
                return new();
            }
            instance = cacheList.Pop(^1).instance;
        }
        if (instance is ICacheable cacheable) { cacheable.IsCached = false; }
        return instance;
    }
    public virtual void Return(T instance)
    {
        if (instance is null) { return; }
        lock (this)
        {
            if (cacheLimit > 0 && cacheList.Count >= cacheLimit) { return; }
            if (instance is ICacheable cacheable) { cacheable.IsCached = true; }
            cacheList.Add((instance, DateTime.Now));
        }
    }
    public void ClearAndTrim()
    {
        lock (this)
        {
            cacheList.Clear();
            cacheList.TrimExcess();
        }
    }
    public void TrimOlder(DateTime older)
    {
        var shouldTrim = false;
        lock (this)
        {
            for (var i = 0; i < cacheList.Count; i++)
            {
                if (cacheList[i].time < older)
                {
                    cacheList.RemoveAt(i);
                    i--;
                    shouldTrim = true;
                }
            }
            if (shouldTrim) { cacheList.TrimExcess(); }
        }
    }
}