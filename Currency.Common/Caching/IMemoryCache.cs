using System;
using System.Collections.Generic;
using System.Text;

namespace Currency.Common.Caching
{
    public interface IMemoryCache
    {
        void Set(string key, string values);

        string GetOrCreate();
    }
}
