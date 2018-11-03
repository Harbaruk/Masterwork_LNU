using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Starter.Services.CacheManager
{
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;

        public CacheManager(IDistributedCache cache)
        {
            _cache = cache;
        }

        public void Delete(string key)
        {
            _cache.Remove(key);
        }

        public T GetValue<T>(string key)
        {
            var value = _cache.GetString(key);

            if (value == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(value);
        }

        public void SetValue(string key, object value)
        {
            _cache.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}