using System.Web.Mvc;

namespace AABC.PatientPortal
{
    public class ViewEngineConfig
    {

        public static void RegisterViewEngine(RazorViewEngine engine) {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine);
        }

    }
}