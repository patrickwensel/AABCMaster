using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;

namespace Dymeng.Framework.Web.Mvc.Views
{
    public class ViewUtilities
    {

        public static string RenderPartialToString(string controlName, object viewData) {
            ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewData);
            viewPage.Controls.Add(viewPage.LoadControl(controlName));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb)) {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw)) {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }


        


    }
}
