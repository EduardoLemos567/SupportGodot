using System.Collections;
using System.Collections.Generic;

namespace Support
{
    public readonly struct ReverseListSegment<T> : IReadOnlyList<T>
    {
        private readonly IList<T> list;
        private readonly int offset;
        private readonly int count;
        public T this[int index]
        {
            get => list[ReverseIndex(index)];
            set => list[ReverseIndex(index)] = value;
        }
        public int Count => count;
        public bool IsReadOnly => true;
        public ReverseListSegment(IList<T> list)
        {
            this.list = list;
            offset = 0;
            count = list.Count;
        }
        public ReverseListSegment(IList<T> list, int offset, int count)
        {
            this.list = list;
            this.offset = offset;
            this.count = count;
        }
        private int ReverseIndex(int index) => offset + (count - 1) - index;
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < count; i++)
            {
                yield return list[ReverseIndex(i)];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
