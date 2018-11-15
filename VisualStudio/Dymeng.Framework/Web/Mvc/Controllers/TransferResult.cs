using System;

using System.Web.Mvc;

namespace Dymeng.Framework.Web.Mvc.Controllers
{

    // http://stackoverflow.com/questions/799511/how-to-simulate-server-transfer-in-asp-net-mvc?lq=1


    public class TransferResult : ActionResult
    {


        public string Url { get; private set; }

        public TransferResult(string url) {
            this.Url = url;
        }

        public override void ExecuteResult(ControllerContext context) {
            
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            var httpContext = System.Web.HttpContext.Current;

            // MVC 3+
            if (System.Web.HttpRuntime.UsingIntegratedPipeline) {
                httpContext.Server.TransferRequest(this.Url, true);
            } else {
                // Pre MVC 3
                httpContext.RewritePath(this.Url, false);

                System.Web.IHttpHandler httpHandler = new MvcHttpHandler();
                httpHandler.ProcessRequest(httpContext);
            }

        }

    }
}
