using System;
using System.Collections;
using System.Collections.Generic;

namespace Support;

public class OrderedSet<T> : ICollection<T> where T : notnull
{
    private readonly Dictionary<T, LinkedListNode<T>> indexed = [];
    private readonly LinkedList<T> ordered = new();
    public int Count => indexed.Count;
    public bool IsReadOnly => false;
    public bool Add(T other)
    {
        if (!indexed.ContainsKey(other))
        {
            indexed.Add(other, ordered.AddLast(other));
            return true;
        }
        return false;
    }
    public bool Remove(T other)
    {
        if (indexed.TryGetValue(other, out var otherNode))
        {
            ordered.Remove(otherNode);
            _ = indexed.Remove(other);
            return true;
        }
        return false;
    }
    public bool Contains(T other) => indexed.ContainsKey(other);
    public bool Overlaps(ISet<T> other) => other.Overlaps(indexed.Keys);
    public IEnumerator<T> GetEnumerator() => ordered.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void TrimExcess() => indexed.TrimExcess();
    public void Clear()
    {
        indexed.Clear();
        ordered.Clear();
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (var item in ordered)
        {
            array[arrayIndex++] = item;
        }
    }
    public void ExceptWith(IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            _ = Remove(item);
        }
    }
    public void UnionWith(IEnumerable<T> other)
    {
        foreach (var item in other)
        {
            _ = Add(item);
        }
    }
    void ICollection<T>.Add(T item) => Add(item);
}
public class ReadOnlyOrderedSet<T> : ICollection<T>, IReadOnlyCollection<T> where T : notnull
{
    private readonly Dictionary<T, LinkedListNode<T>> indexed = [];
    private readonly LinkedList<T> ordered = new();
    public ReadOnlyOrderedSet(in IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            indexed.Add(item, ordered.AddLast(item));
        }
    }
    public int Count => indexed.Count;
    public bool IsReadOnly => true;
    public void Add(T item) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(T item) => indexed.ContainsKey(item);
    public bool Overlaps(ISet<T> other) => other.Overlaps(indexed.Keys);
    public void CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    public IEnumerator<T> GetEnumerator() => ordered.GetEnumerator();
    public bool Remove(T item) => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}