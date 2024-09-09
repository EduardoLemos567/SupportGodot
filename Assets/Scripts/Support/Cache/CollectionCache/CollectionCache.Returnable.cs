using System;
using System.Collections.Generic;

namespace Support.Cache
{
    public partial class CollectionCache<TCollection, T> where TCollection : ICollection<T>, new()
    {
        public readonly struct Returnable : IDisposable
        {
            private readonly CollectionCache<TCollection, T> Cache;
            public readonly TCollection Instance;
            public Returnable(CollectionCache<TCollection, T> cache, TCollection instance)
            {
                Cache = cache;
                Instance = instance;
            }
            public void Dispose() => Cache.Return(Instance);
        }
    }
}