using System.Collections.Generic;
using System.Linq;

namespace AABC.PatientPortal.App.Shared
{
    public class Navigation
    {


        public enum NavigationKey
        {
            Home,
            Settings,
            Payments
        }


        /// <summary>
        /// For the given request path, retrieve all of the matched navigation keys
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public NavigationKey[] GetActiveKeys(string path) {

            if (path.Substring(0, 1) != "/") {
                path = "/" + path;
            }

            PathKeys keys = GetPathKeys().Where(x => x.Path == path).SingleOrDefault();

            return keys.Keys;
        }


        private PathKeys[] LoadPathKeys() {

            var keys = new List<PathKeys>
            {
                new PathKeys("/", new NavigationKey[] { NavigationKey.Home }),
                new PathKeys("/Payments", new NavigationKey[] { NavigationKey.Payments }),
                new PathKeys("/Settings", new NavigationKey[] { NavigationKey.Settings })
            };
            return keys.ToArray();
        }

        private PathKeys[] GetPathKeys() {

            object keys = CacheService.Current.App.Get(CacheService.Types.AppCache.Key.NavigationPathKeys);
            if (keys == null) {
                keys = LoadPathKeys();
                CacheService.Current.App.Add(CacheService.Types.AppCache.Key.NavigationPathKeys, keys);
            }
            return keys as PathKeys[];
        }


        private struct PathKeys
        {

            public PathKeys(string path, NavigationKey[] keys) {
                Path = path;
                Keys = keys;
            }

            public string Path { get; set; }
            public NavigationKey[] Keys { get; set; }
        }

    }
}