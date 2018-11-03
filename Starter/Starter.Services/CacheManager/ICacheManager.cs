using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Services.CacheManager
{
    public interface ICacheManager
    {
        void SetValue(string key, object value);

        T GetValue<T>(string key);

        void Delete(string key);
    }
}