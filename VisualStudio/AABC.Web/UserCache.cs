using System;

using System.Runtime.Caching;

namespace AABC.Web
{
    static class UserCache
    {


        const int CACHE_TIMEOUT_MINUTES = 20;

        private static int userID {
            get
            {
                return Global.Default.User().ID.Value;
            }
        }
        
        private static string getKey(string key) {
            return userID.ToString() + "_" + key;
        }

        public static void AddItem(string key, object item) {
            MemoryCache c = MemoryCache.Default;
            c.Set(getKey(key), item, DateTimeOffset.Now.AddMinutes(CACHE_TIMEOUT_MINUTES));
        }

        public static object GetItem(string key) {
            return MemoryCache.Default.Get(getKey(key));
        }

        public static void InvalidateItem(string key) {
            MemoryCache.Default.Remove(getKey(key));
        }

        public static void InvalidateAll() {
            MemoryCache.Default.Trim(100);
        }



    }
}