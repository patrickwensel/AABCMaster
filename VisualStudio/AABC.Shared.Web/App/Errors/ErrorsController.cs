using System;
using System.Web.Mvc;

namespace AABC.Shared.Web.App.Errors
{
    public class ErrorsControllerBase : Controller
    {

        [HttpPost]
        [AllowAnonymous]        
        public ActionResult LogJS(string message, string source, int? lineno, int? colno, string jsonError) {

            string exceptionMessage = "";

            exceptionMessage += "DATE: " + DateTime.Now.ToString();
            exceptionMessage += "MESSAGE: " + message ?? "" + Environment.NewLine;
            exceptionMessage += "SOURCE: " + source ?? "" + Environment.NewLine;
            exceptionMessage += "LINENO: " + lineno.GetValueOrDefault() + Environment.NewLine;
            exceptionMessage += "COLNO: " + colno.GetValueOrDefault() + Environment.NewLine;
            exceptionMessage += " JSONERROR: " + jsonError ?? "" + Environment.NewLine;

            var e = new JavaScriptErrorException(exceptionMessage);

            Dymeng.Framework.Exceptions.Handle(e);

            return new HttpStatusCodeResult(200);
        }



        private class JavaScriptErrorException : Exception {
            public JavaScriptErrorException() : base() { }
            public JavaScriptErrorException(string message) : base(message) { }
        }

    }
}
