using System;

namespace Support.Cache;

public partial class ArrayCache<T>
{
    public readonly struct Returnable : IDisposable
    {
        private readonly ArrayCache<T> Cache;
        public readonly T[] Array;
        public Returnable(ArrayCache<T> cache, T[] array)
        {
            Cache = cache;
            Array = array;
        }
        public void Dispose() => Cache.Return(Array);
    }
}