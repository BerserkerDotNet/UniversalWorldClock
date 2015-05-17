using System;
using System.Collections.Generic;

namespace UniversalWorldClock.Services
{
    public class SimpleMemoryCacheClient:ICacheClient
    {
        private readonly Dictionary<string, WeakReference<object>> _cacheStorage = new Dictionary<string, WeakReference<object>>(10); 

        public T Get<T>(string key)
        {
            object data;
            if (_cacheStorage.ContainsKey(key) && _cacheStorage[key].TryGetTarget(out data))
                return (T) data;

            return default(T);
        }

        public void Set(string key, object data)
        {
            _cacheStorage[key] = new WeakReference<object>(data);
        }
    }
}