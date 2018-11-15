using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Web;
using System.Web.Mvc;

namespace AABC.ProviderPortal
{

    /// <summary>
    /// Use in place of return Json() to return a properly formatted date
    /// </summary>
    public class DateFormattedJsonResult : System.Web.Mvc.JsonResult
    {

        /* Usage:
         * 
         *    [HttpGet]
         *    Public JsonResult Method() {
         *        return new DateFormattedJsonResult { Data = yourDataObject };
         *    }
         */

        private const string _dateFormat = "yyyy-MM-ddTHH:mm:ss";

        public override void ExecuteResult(ControllerContext context) {

            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType)) {
                response.ContentType = ContentType;
            } else {
                response.ContentType = "application/json";
            }

            if (ContentEncoding != null) {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null) {

                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }
}