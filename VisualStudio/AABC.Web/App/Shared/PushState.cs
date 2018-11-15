using System;

namespace AABC.Web.App.Shared
{
    public class PushState
    {
        public string Route { get; set; }
        public string Title { get; set; }
        [Obsolete]
        public int GroupIndex { get; set; }
        [Obsolete]
        public int GroupItemIndex { get; set; }

        public string NavBarRoute { get; set; }
    }
}