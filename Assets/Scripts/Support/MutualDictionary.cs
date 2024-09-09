using System.Collections;
using System.Collections.Generic;
using Support.Diagnostics;

namespace Support;

public class MutualDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>
    where K : notnull
    where V : notnull
{
    private readonly Dictionary<K, V> keyValue;
    private readonly Dictionary<V, K> valueKey;
    public int Count => keyValue.Count;
    public V this[K key] => keyValue[key];
    public K this[V value] => valueKey[value];
    public MutualDictionary(int initialCapacity = 16)
    {
        Debug.Assert(typeof(K) != typeof(V), $"Key and Value types must be different");
        keyValue = new(initialCapacity);
        valueKey = new(initialCapacity);
    }
    public void Add(in K key, in V value)
    {
        keyValue.Add(key, value);
        valueKey.Add(value, key);
    }
    public void RemoveByKey(in K key)
    {
        if (keyValue.TryGetValue(key, out var value))
        {
            _ = keyValue.Remove(key);
            _ = valueKey.Remove(value);
        }
    }
    public void RemoveByValue(in V value)
    {
        if (valueKey.TryGetValue(value, out var key))
        {
            _ = valueKey.Remove(value);
            _ = keyValue.Remove(key);
        }
    }
    public bool TryGetValue(in K key, out V? value) => keyValue.TryGetValue(key, out value);
    public bool TryGetKey(in V value, out K? key) => valueKey.TryGetValue(value, out key);
    public bool ContainsKey(in K key) => keyValue.ContainsKey(key);
    public bool ContainsValue(in V value) => valueKey.ContainsKey(value);
    public void Clear()
    {
        keyValue.Clear();
        valueKey.Clear();
    }
    public IEnumerable<K> Keys => keyValue.Keys;
    public IEnumerable<V> Values => keyValue.Values;
    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => keyValue.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}