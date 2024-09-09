using System;

namespace Support.Cache;

public partial class ArrayCache<T>
{
    public readonly struct ReturnableSegment : IDisposable
    {
        private readonly ArrayCache<T> Cache;
        public readonly ArraySegment<T> ArraySegment;
        public ReturnableSegment(ArrayCache<T> cache, ArraySegment<T> segment)
        {
            Cache = cache;
            ArraySegment = segment;
        }
        public void Dispose() => Cache.Return(ArraySegment.Array!);
    }
}