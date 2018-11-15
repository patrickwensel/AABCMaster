using System.Web.Optimization;

namespace AABC.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/Scripts/underscore.min.js",
                        "~/Scripts/mustache.js",
                        "~/Scripts/ko/knockout.js",
                        "~/Scripts/ko/knockout.mapping-latest.js",
                        "~/Scripts/ko/knockout.validation.min.js",
                        "~/Scripts/ko/bindings/*.js",
                        "~/Scripts/numeral/numeral.min.js",
                        "~/Scripts/datepicker/bootstrap-datepicker.min.js",
                        "~/Scripts/cleave/cleave.min.js",
                        "~/Scripts/moment.js",
                        "~/Scripts/Pages/AdminPatientPortal.js",
                        "~/Scripts/Pages/CaseManageInsurance.js",
                        "~/Scripts/Pages/CaseManagePayment.js",
                        "~/Scripts/Pages/HoursMaxPerDayPerPatient.js",
                        "~/Scripts/Pages/PatientSelect.js",
                        "~/Scripts/Pages/ReportingAuthorizations.js",
                        "~/Scripts/Pages/AdminPatientPortal.js",
                        "~/Scripts/Pages/CaseManageProviders2.js",
                        "~/Scripts/Pages/ProviderSubTypes.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/hoursEntry").Include(
                        "~/client/scripts/app/hoursEntry/hoursEntry.js",
                        "~/client/scripts/app/hoursEntry/hoursNotes.js",
                        "~/client/scripts/app/hoursEntry/hoursServices.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/highcharts").Include(
                        "~/Scripts/highcharts/highcharts.js",
                        "~/Scripts/highcharts/modules/exporting.js"
                ));
        }
    }
}
