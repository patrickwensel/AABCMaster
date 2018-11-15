using Dymeng.Framework.Web.Mvc.Views;
using System.Web.Mvc;


namespace AABC.Web.App.Services
{

    [Authorize]
    public class ServicesController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        public ActionResult Index()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Services/Index", "Services");
            return GetView("Index");
        }
        


        private ServicesService _service;

        public ServicesController() {
            _service = new ServicesService(AppService.Current.DataContextV2);
        }

    }
}