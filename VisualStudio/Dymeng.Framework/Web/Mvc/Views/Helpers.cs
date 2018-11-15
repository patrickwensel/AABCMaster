namespace Dymeng.Framework.Web.Mvc.Views
{
    public static class Helpers
    {


        public static string GetJavaScriptString(string input) {

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(input);

        }

    }
}
