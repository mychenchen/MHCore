using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Currency.Common.Caching
{
    public class CoreMemoryCache
    {
        private IMemoryCache _cache;
        public CoreMemoryCache(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        /// <summary>
        /// 存在创建不存在获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetOrCreate(string key, object value)
        {
            if (!string.IsNullOrEmpty(key))
            {
                // public static TItem GetOrCreate<TItem>(this IMemoryCache cache, object key, Func<ICacheEntry, TItem> factory);
                _cache.GetOrCreate(key, ENTRY => { return value; });
            }
            //如果添加成功则验证是否存在
            return Exists(key);
        }

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <param name="times">时间/分钟-默认:30 </param>
        /// <returns></returns>
        public bool Set(string key, object values, int times = 30)
        {
            if (!string.IsNullOrEmpty(key))
            {
                MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
                cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(times);
                cacheExpirationOptions.Priority = CacheItemPriority.Normal;
                //cacheExpirationOptions.RegisterPostEvictionCallback(IDGCacheItemChangedHandler, this);
                //添加类似 System.Web.HttpRuntime.Cache[key] 
                _cache.Set(key, values, cacheExpirationOptions);
            }
            //如果添加成功则验证是否存在返回True 或false
            return Exists(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                //删除
                _cache.Remove(key);

                //如果删除成功则验证是否存在返回bool
                return !Exists(key);
            }
            return false;
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="times">时间/分钟-默认:30 </param>
        /// <returns></returns>
        /// 修改时 MemoryCache 没有提供相对相应的方法，先删除后添加
        public bool Modify(string key, object value, int times = 30)
        {
            bool ReturnBool = false;
            if (!string.IsNullOrEmpty(key))
            {
                if (Exists(key))
                {
                    //删除
                    if (!Remove(key))
                    {
                        ReturnBool = Set(key, value);
                    }
                }

            }
            return ReturnBool;
        }

        /// <summary>
        /// 获取缓存 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (Exists(key))
            {
                return _cache.Get(key).ToJson();
            }
            return default;
        }

        /// <summary>
        /// 验证缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            object ReturnValue;
            if (!string.IsNullOrEmpty(key))
            {
                return _cache.TryGetValue(key, out ReturnValue);
            }
            return false;
        }
    }
}
