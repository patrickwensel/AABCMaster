using System;

using System.Runtime.Caching;

namespace AABC.Data.Services
{

    public enum CacheServiceItems
    {
        CommonLanguageList,
        ProviderTypeList,
        GuardianRelationship,
        ProviderServices
    }


    public class CacheService
    {

        /**
         * Cache can work off Static or explicit keys.
         * Static Keys are used via enum, explicit can be whatever.
         * 
         *************/

        const int DEFAULT_CACHE_MINUTES = 5;
        const string STATIC_KEY_PREFIX = "static_key_id_";

        public static void Add(string key, object item) {
            add(key, item, DEFAULT_CACHE_MINUTES);
        }

        public static void Add(string key, object item, int expireMinutes) {
            add(key, item, expireMinutes);
        }

        public static void Add(CacheServiceItems key, object item) {
            add(getStaticKey(key), item, DEFAULT_CACHE_MINUTES);
        }
        
        public static void Add(CacheServiceItems key, object item, int expireMinutes) {
            add(getStaticKey(key), item, expireMinutes);
        }

        public static object Get(CacheServiceItems key) {
            return get(getStaticKey(key));
        }

        public static object Get(string key) {
            return get(key);
        }
        
        static object get(string key) {
            MemoryCache mc = MemoryCache.Default;
            return mc.Contains(key) ? mc[key] : null;
        }
        
        public static void Invalidate(CacheServiceItems key) {
            invalidate(getStaticKey(key));
        }

        public static void Invalidate(string key) {
            invalidate(key);
        }

        static void invalidate(string key) {
            MemoryCache mc = MemoryCache.Default;
            if (mc.Contains(key)) {
                mc.Remove(key);
            }
        }

        static string getStaticKey(CacheServiceItems key) {
            return STATIC_KEY_PREFIX + ((int)key).ToString();
        }

        static void add(string key, object item, int expirationMinutes) {
            invalidate(key);
            MemoryCache.Default.Add(key, item, DateTimeOffset.UtcNow.AddMinutes(expirationMinutes));
        }


    }
}
