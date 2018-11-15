using System;
using System.Web.Mvc;

namespace AABC.Shared.Web.App.Test
{
    public abstract class TestControllerBase : Controller 
    {

        [HttpGet]
        public ActionResult GlobalErrorLogger() {

            int x = 0;
            int y = 0;

            return Content((x / y).ToString());

        }

        [HttpGet]
        public ActionResult LocalErrorLogger() {

            int x = 0;
            int y = 0;

            string result = "";

            try {
                result = (x / y).ToString();
            } catch (Exception e) {
                Dymeng.Framework.Exceptions.Handle(e);
            }

            return Content(result);
        }

    }
}
