using System.Collections.Generic;

namespace Dymeng.Framework.Web.Mvc.Views
{

    public enum GridTitlePanelFilterItemsDisplay
    {
        HorizontalList,
        Dropdown
    }


    public class GridTitlePanelSettings
    {


        public string Title { get; set; }
        public bool ShowAddButton { get; set; }
        public string AddNewController { get; set; }
        public string AddNewAction { get; set; }

        public GridTitlePanelFilterItemsDisplay FilterItemsDisplayMode { get; set; }

        public List<GridTitlePanelFilterItem> FilterItems { get; set; }



        // CTORs
        public GridTitlePanelSettings() {
            FilterItems = new List<GridTitlePanelFilterItem>();
            ShowAddButton = true;
            init();
        }

        public GridTitlePanelSettings(string title) {
            Title = title;
            FilterItems = new List<GridTitlePanelFilterItem>();
            ShowAddButton = true;
            init();
        }

        public GridTitlePanelSettings(string title, bool showAddButton) {
            Title = title;
            FilterItems = new List<GridTitlePanelFilterItem>();
            ShowAddButton = showAddButton;
            init();
        }

        public GridTitlePanelSettings(string title, List<GridTitlePanelFilterItem> filterItems) {
            Title = title;
            FilterItems = filterItems;
            ShowAddButton = true;
            init();
        }

        public GridTitlePanelSettings(string title, bool showAddButton, List<GridTitlePanelFilterItem> filterItems) {
            Title = title;
            FilterItems = filterItems;
            ShowAddButton = showAddButton;
            init();
        }

        void init() {
            FilterItemsDisplayMode = GridTitlePanelFilterItemsDisplay.HorizontalList;
        }



    }


    public class GridTitlePanelFilterItem
    {
        public string RouteUrl { get; set; }
        public string Text { get; set; }
        public string Script { get; set; }
        public bool Enabled { get; set; }
        public bool IsActive { get; set; }
    }

}
