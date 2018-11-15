namespace Dymeng.Framework.Web.Mvc.Views
{

    public interface IViewModelListBase
    {
        string CallbackFilterValue { get; set; }
        GridTitlePanelSettings GridTitlePanelSettings { get; set; }
    }


    public class ViewModelListBase : IViewModelListBase
    {

        public string CallbackFilterValue { get; set; }
        public GridTitlePanelSettings GridTitlePanelSettings { get; set; }

        public ViewModelListBase()
        {
            GridTitlePanelSettings = new GridTitlePanelSettings();
        }

        public ViewModelListBase(GridTitlePanelSettings gridTitlePanelSettings)
        {
            GridTitlePanelSettings = gridTitlePanelSettings;
        }

    }
}
