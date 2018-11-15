namespace Dymeng.Framework.Web.Mvc.Views
{
    public interface IWebViewHelper
    {

        void BindModel();
        bool Validate();
        bool HasValidationErrors { get; set; }
        string ReturnErrorMessage { get; set; }
        //void Save();

    }
}
