using System;
using System.Collections;
using System.Collections.Generic;

namespace Support
{
    public readonly struct ReadOnlySet<T> : IReadOnlyCollection<T>, ISet<T>
    {
        private readonly ISet<T> set;
        public readonly int Count => set.Count;
        public readonly bool IsReadOnly => true;
        public ReadOnlySet(ISet<T> set) => this.set = set;
        public readonly IEnumerator<T> GetEnumerator() => set.GetEnumerator();
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<T>.Add(T item) => throw new NotSupportedException("Set is a read only.");
        public readonly void UnionWith(IEnumerable<T> other) => throw new NotSupportedException("Set is a read only.");
        public readonly void IntersectWith(IEnumerable<T> other) => throw new NotSupportedException("Set is a read only.");
        public readonly void ExceptWith(IEnumerable<T> other) => throw new NotSupportedException("Set is a read only.");
        public readonly void SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException("Set is a read only.");
        public readonly bool IsSubsetOf(IEnumerable<T> other) => set.IsSubsetOf(other);
        public readonly bool IsSupersetOf(IEnumerable<T> other) => set.IsSupersetOf(other);
        public readonly bool IsProperSupersetOf(IEnumerable<T> other) => set.IsProperSupersetOf(other);
        public readonly bool IsProperSubsetOf(IEnumerable<T> other) => set.IsProperSubsetOf(other);
        public readonly bool Overlaps(IEnumerable<T> other) => set.Overlaps(other);
        public readonly bool Add(T item) => throw new NotImplementedException("Set is a read only.");
        public readonly bool SetEquals(IEnumerable<T> other) => set.SetEquals(other);
        public readonly void Clear() => throw new NotImplementedException("Set is a read only.");
        public readonly bool Contains(T item) => set.Contains(item);
        public readonly void CopyTo(T[] array, int arrayIndex) => set.CopyTo(array, arrayIndex);
        public readonly bool Remove(T item) => throw new NotImplementedException("Set is a read only.");
        public static implicit operator ReadOnlySet<T>(in HashSet<T> set) => new(set);
    }
}