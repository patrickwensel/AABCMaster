using AABC.DomainServices.Notes;
using AABC.Web.App.Home;
using AABC.Web.App.Models;
using AABC.Web.App.Notes;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Web.Mvc;

namespace AABC.Web.Home
{
    public class HomeController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        public ActionResult Index()
        {
            return RedirectToAction("Tasks");
        }


        [Authorize]
        public ActionResult Tasks()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Home/Tasks", "Tasks");
            return GetView("Tasks");
        }


        public ActionResult TaskUserDashboardTodoGrid()
        {
            var tasks = TaskService.GetAllIncompleteTasks();
            return PartialView(tasks);
        }


        public ActionResult TaskUserDashboardCompletedGrid()
        {
            var tasks = TaskService.GetRecentlyCompletedTasks();
            return PartialView(tasks);
        }


        [Authorize]
        public ActionResult HoursEligibility()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Home/HoursEligibility", "Hours Eligibility"); ;
            return GetView("HoursEligibility");
        }


        [Authorize]
        [Route("Home/HoursEligibility/GetModel")]
        public JsonResult HoursEligibilityGetModel()
        {
            var model = App.Home.Models.HoursEligibility.HoursEligiblityVM.GetModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        public ActionResult Dashboard()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Home/Dashboard", "Dashboard");
            var user = Global.Default.User();
            if (user.UserName == "Yakov" || user.UserName == "jleach" || user.UserName == "kim")
            {
                return DashboardYakov();
            }
            return RedirectToAction("Tasks");
        }


        public ActionResult DashboardYakov()
        {
            //if (Global.Default.User().UserName != "Yakov" && Global.Default.User().UserName != "jleach") {
            //    throw new HttpException(403, "Forbidden");
            //}
            var model = new DashboardYakovVM
            {
                AvailableDates = DashboardService.GetAvailableDates(),
                SelectedDate = DashboardService.DefaultSelectedDate(),
                InsuranceList = DashboardService.GetInsuranceList(),
                Insurance2List = DashboardService.GetInsurance2List()
            };
            model.InsuranceCostsByPatient = DashboardService.InsuranceCostsListItems(model.SelectedDate.Date, "GHI");
            return GetView("DashboardYakov", model);
        }


        public ActionResult DashboardYakovGetInsuranceCosts(DateTime period, string insuranceName, int insuranceID)
        {
            var model = DashboardService.Insurance2CostsListItems(period, insuranceID);
            //model = service.InsuranceCostsListItems(period, insuranceName);
            return PartialView("DashboardYakovInsCostList", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DashboardYakovInsCostGridCallback()
        {
            DateTime period = DateTime.Parse(Request.Params["period"]);
            string insuranceName = Request.Params["insuranceName"];
            int insuranceID = int.Parse(Request.Params["insuranceID"]);
            var model = DashboardService.Insurance2CostsListItems(period, insuranceID);
            return PartialView("DashboardYakovInsCostList", model);
        }


        public ActionResult NavigateBack(string routeUrl)
        {
            // intended for use on ajax call from onpopstate
            // currently handles urls without any query param (adds ?navigateBack=true)
            if (string.IsNullOrEmpty(Request.Url.Query))
            {
                return Redirect(routeUrl + "?navigateBack=true");
            }
            else
            {
                return Redirect(routeUrl + "&navigateBack=true");
            }
            // TEST: check pre-existing querystring append correct
        }


        public ActionResult Navigate(string toController, string toAction)
        {
            try
            {
                if (string.IsNullOrEmpty(toController) || string.IsNullOrEmpty(toAction))
                {
                    return PartialView("ErrorPartial");
                }
                return new Dymeng.Framework.Web.Mvc.Controllers.TransferResult("/" + toController + "/" + toAction);
            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                return PartialView("ErrorPartial");
            }
        }


        private readonly DashboardService DashboardService;
        private readonly TaskService TaskService;

        public HomeController()
        {
            DashboardService = new DashboardService();
            TaskService = new TaskService(AppService.Current.DataContextV2, new UserProvider());
        }

    }
}