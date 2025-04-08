using System;
using System.Collections.Generic;
using System.Threading;

namespace Support.Cache;

/// <summary>
/// Cache data by size or time, or both.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
public class DictionaryCache<K, V> where K : notnull
{
    private readonly struct Data
    {
        public K Key { get; init; }
        public V Value { get; init; }
        public DateTime Time { get; init; }
    }

    private readonly Dictionary<K, LinkedListNode<Data>> dataMapper = new();
    private readonly LinkedList<Data> dataList = new();

    private readonly Action<K, V>? onEvict;

    public TimeSpan? MaxOld { get; }
    public int? MaxSize { get; }
    public int Count => dataMapper.Count;

    public DictionaryCache(TimeSpan? maxOld = null, int? maxSize = null, Action<K, V>? onEvict = null)
    {
        MaxOld = maxOld;
        MaxSize = maxSize;
        this.onEvict = onEvict;
    }

    public void Add(in K key, in V value)
    {
        Monitor.Enter(this);

        if (MaxSize.HasValue && dataMapper.Count >= MaxSize.Value || ContainsKey(key))
        {
            Monitor.Exit(this);
            Remove(dataList.First!.Value.Key);
            onEvict?.Invoke(dataList.First!.Value.Key, dataList.First!.Value.Value);
            Monitor.Enter(this);
        }

        LinkedListNode<Data> node = dataList.AddLast(new Data { Key = key, Value = value, Time = DateTime.Now });
        dataMapper.Add(key, node);

        Monitor.Exit(this);
    }

    public bool TryGetValue(in K key, out V? value)
    {
        lock (this)
        {
            if (dataMapper.TryGetValue(key, out LinkedListNode<Data>? node))
            {
                value = node.Value.Value;
                return true;
            }

            value = default;
            return false;
        }
    }

    public void Remove(in K key)
    {
        lock (this)
        {
            if (dataMapper.TryGetValue(key, out LinkedListNode<Data>? node))
            {
                dataList.Remove(node);
                dataMapper.Remove(key);
            }
        }
    }

    public bool TryPop(in K key, out V? value)
    {
        lock (this)
        {
            if (dataMapper.TryGetValue(key, out LinkedListNode<Data>? node))
            {
                dataList.Remove(node);
                dataMapper.Remove(key);
                value = node.Value.Value;
                return true;
            }

            value = default;

            return false;
        }
    }

    public void ClearStale()
    {
        if (!MaxOld.HasValue)
            return;

        DateTime now = DateTime.Now;

        Monitor.Enter(this);

        while (dataList.Count > 0 && now - dataList.First!.Value.Time > MaxOld.Value)
        {
            Monitor.Exit(this);
            Remove(dataList.First!.Value.Key);
            Monitor.Enter(this);
        }

        Monitor.Exit(this);
    }

    public void ClearAll()
    {
        lock (this)
        {
            dataMapper.Clear();
            dataList.Clear();
        }
    }

    public bool ContainsKey(in K key)
    {
        lock (this)
        {
            return dataMapper.ContainsKey(key);
        }
    }
}
