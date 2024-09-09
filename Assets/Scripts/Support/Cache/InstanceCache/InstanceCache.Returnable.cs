using System;

namespace Support.Cache
{
    public partial class InstanceCache<T> where T : class, new()
    {
        public readonly struct Returnable : IDisposable
        {
            private readonly InstanceCache<T> Cache;
            public readonly T Instance;
            public Returnable(InstanceCache<T> cache, T instance)
            {
                Cache = cache;
                Instance = instance;
            }
            public void Dispose() => Cache.Return(Instance);
        }
    }
}