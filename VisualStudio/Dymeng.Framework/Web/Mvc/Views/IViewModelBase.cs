namespace Dymeng.Framework.Web.Mvc.Views
{
    public interface IViewModelBase
    {

        bool PushState { get; set; }
        string PushStateRoute { get; set; }
        string PushStateTitle { get; set; }

        string NavBarRoute { get; set; }
    }

    public class ViewModelBase : IViewModelBase
    {
        public bool PushState { get; set; }
            
        public string PushStateRoute { get; set; }

        public string PushStateTitle { get; set; }

        public string NavBarRoute { get; set; }

        public ViewModelBase() { }

        public ViewModelBase(bool pushState, string route, string title)
        {
            PushStateRoute = NavBarRoute = route;
            PushStateTitle = title;
            PushState = pushState;
        }
        
        public ViewModelBase(bool pushState, string route, string title, string navBarRoute) {
            PushStateRoute = route;
            PushStateTitle = title;
            PushState = pushState;
            NavBarRoute = navBarRoute;
        }

    }
}
