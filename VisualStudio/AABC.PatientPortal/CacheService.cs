using System;
using System.Runtime.Caching;
using System.Web;

namespace AABC.PatientPortal
{
    public class CacheService
    {

        const int CACHE_TIMEOUT_MINUTES = 20;

        private static CacheService _current;

        public static CacheService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new CacheService();
                }
                return _current;
            }
        }

        public Types.AppCache App { get; set; }
        public Types.UserCache User { get; set; }

        public CacheService()
        {
            App = new Types.AppCache(this);
            User = new Types.UserCache(this);
        }


        public class Types
        {

            public class AppCache
            {

                public enum Key
                {
                    NavigationPathKeys
                }

                private const string KEY_PREFIX = "AppCache_";
                private readonly CacheService _service;
                private MemoryCache _cache { get { return MemoryCache.Default; } }

                public AppCache(CacheService service)
                {
                    _service = service;
                }

                public void Add(Key key, object obj)
                {
                    string k = GetKey(key);
                    _cache.Set(k, obj, GetExpiration(CACHE_TIMEOUT_MINUTES));
                }

                public void Remove(Key key)
                {
                    _cache.Remove(GetKey(key));
                }

                public void RemoveAll()
                {
                    foreach (Key key in Enum.GetValues(typeof(Key)))
                    {
                        Remove(key);
                    }
                }

                public object Get(Key key)
                {
                    return _cache.Get(GetKey(key));
                }





                private string GetKey(Key key)
                {
                    return key.ToString();
                }

                private DateTimeOffset GetExpiration(int minutes)
                {
                    return DateTimeOffset.Now.AddMinutes(minutes);
                }


            }

            public class UserCache
            {

                /*
                 * This cache uses the session id as a differentiator between users.
                 * To ensure the session ID is handled properly, make sure the global.asax
                 * has the Session_Start event and puts some dummy value into it in order
                 * for it to persist correctly.  Also, note that if httpCookies requireSSL
                 * is on, the session will be recreated per request if accessed without
                 * https.
                 * 
                 * Ref: http://stackoverflow.com/questions/2874078/asp-net-session-sessionid-changes-between-requests
                 * 
                 */

                private const string KEY_PREFIX = "UserCache_";
                private MemoryCache _cache { get { return MemoryCache.Default; } }
                private readonly CacheService _service;

                public UserCache(CacheService service)
                {
                    _service = service;
                }

                public enum Key
                {
                    User
                }

                public void Add(Key key, object obj)
                {
                    string k = GetKey(key);
                    _cache.Set(k, obj, GetExpiration(CACHE_TIMEOUT_MINUTES));
                }

                public void Remove(Key key)
                {
                    _cache.Remove(GetKey(key));
                }

                public void RemoveAll()
                {
                    foreach (Key key in Enum.GetValues(typeof(Key)))
                    {
                        Remove(key);
                    }
                }

                public object Get(Key key)
                {
                    return _cache.Get(GetKey(key));
                }




                private string GetKey(Key key)
                {
                    string s = KEY_PREFIX + key.ToString() + "_" + GetSessionID();
                    return s;
                }

                private string GetSessionID()
                {
                    return HttpContext.Current.Session.SessionID;
                }

                private DateTimeOffset GetExpiration(int minutes)
                {
                    return DateTimeOffset.Now.AddMinutes(minutes);
                }


            }

        }




    }
}