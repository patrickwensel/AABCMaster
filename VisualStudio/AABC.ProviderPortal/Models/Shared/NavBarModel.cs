using System;

namespace AABC.ProviderPortal.Models.Shared
{
    public class NavBarModel
    {
        public NavBarGroupHome Home { get; }
        
        public NavBarModel() {
            Home = new NavBarGroupHome();
        }

        public static NavBarModel GetNavBarModel() {

            var memoryCache = System.Runtime.Caching.MemoryCache.Default;

            if (!memoryCache.Contains("PPNavBarModel")) {
                var expiration = DateTimeOffset.UtcNow.AddHours(1);
                var navBarModel = new NavBarModel();

                memoryCache.Add("PPNavBarModel", navBarModel, expiration);
            }

            return memoryCache.Get("PPNavBarModel", null) as NavBarModel;
        }


        

        

        public void GetRouteInfo(int group, int item, out string controllerName, out string actionName) {

            controllerName = "";
            actionName = "";

            switch (group) {
                case 0: // cases
                    controllerName = "Home";
                    if (item == 0)
                        actionName = "Cases";
                    break;
                    
                default:
                    throw new ArgumentOutOfRangeException("Group or Item index not recognized");
            }
        }
        
    }

    public class NavBarGroupHome
    {
        public int GroupIndex { get { return 0; } }
        public int Cases { get { return 0; } }
    }
    
}