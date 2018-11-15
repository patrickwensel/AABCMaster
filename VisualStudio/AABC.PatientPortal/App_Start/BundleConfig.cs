using AABC.PatientPortal.Helpers;
using System.Web.Optimization;

namespace AABC.PatientPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {

            //BundleTable.EnableOptimizations = true;

            // Core scripts
            bundles.Add(new ScriptBundle("~/bundles/core").IncludeExisting(
                        "~/client/scripts/lib/jquery-2.1.1.min.js",
                        "~/client/scripts/lib/knockout-3.4.2.js",
                        "~/client/scripts/lib/knockout.mapping-latest.js",
                        "~/client/scripts/lib/moment.2.17.1.min.js",
                        "~/client/scripts/lib/jquery.validate.min.js"));
            
            // bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").IncludeExisting(
                      "~/client/scripts/lib/bootstrap.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").IncludeExisting(
                      "~/client/scripts/plugins/slimScroll/jquery.slimscroll.min.js",
                      "~/client/scripts/plugins/metisMenu/metisMenu.min.js",
                      "~/client/scripts/plugins/pace/pace.min.js",
                      "~/client/scripts/plugins/toastr/toastr.min.js",
                      "~/client/scripts/plugins/jasny/jasny-bootstrap.min.js",
                      "~/client/scripts/plugins/datepicker/bootstrap-datepicker.js",
                      "~/client/scripts/extensions/ko.bindingHandlers.datepicker.js",
                      "~/client/scripts/plugins/moment/moment.js",
                      "~/client/scripts/plugins/numeral/numeral.js",
                      "~/client/scripts/plugins/knockout-validation/knockout.validation.js",
                      "~/client/scripts/lib/inspinia.js"));

            
            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").IncludeExisting(
                      "~/client/styles/bootstrap.min.css",
                      "~/client/styles/animate.css",
                      "~/client/styles/style.css",
                      "~/client/styles/app.v2.css",
                      "~/client/styles/plugins/datepicker/bootstrap-datepicker3.min.css"));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").IncludeExisting(
                      "~/client/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));
            
            // toastr notification styles
            bundles.Add(new StyleBundle("~/plugins/toastrStyles").IncludeExisting(
                      "~/client/styles/plugins/toastr/toastr.min.css"));


            // jasny
            bundles.Add(new StyleBundle("~/plugins/jasnyBootstrapStyles").IncludeExisting(
                    "~/client/styles/plugins/jasny/jasny-bootstrap.min.css"));

            
            // Dymeng scripts
            bundles.Add(new ScriptBundle("~/lib/dymeng").IncludeExisting(
                "~/client/scripts/lib/dymeng.v2.js"
                ));

            bundles.Add(new ScriptBundle("~/app/app").IncludeExisting(
                "~/client/scripts/app/app.v2.js"
                ));
                        
            // signature Pad
            bundles.Add(new ScriptBundle("~/lib/signaturePad").IncludeExisting(
                "~/client/scripts/lib/signature_pad.min.js"));

            // Ladda buttons Styless
            bundles.Add(new StyleBundle("~/plugins/laddaStyles").Include(
                      "~/client/styles/plugins/ladda/ladda-themeless.min.css"));

            // Ladda buttons
            bundles.Add(new ScriptBundle("~/plugins/ladda").Include(
                      "~/client/scripts/plugins/ladda/spin.min.js",
                      "~/client/scripts/plugins/ladda/ladda.min.js",
                      "~/client/scripts/plugins/ladda/ladda.jquery.min.js"));





            // App Specific

            bundles.Add(new ScriptBundle("~/app/settings").Include(
                "~/client/scripts/app/settings.v2.js"));

            bundles.Add(new ScriptBundle("~/app/home").Include(
                "~/client/scripts/app/home.v3.js"));


            bundles.Add(new ScriptBundle("~/app/payments").Include(
                "~/client/scripts/app/payments.v1.js"));

            bundles.Add(new ScriptBundle("~/app/payments.history").Include(
                "~/client/scripts/app/payments.history.v1.js"));

            bundles.Add(new ScriptBundle("~/app/payments.scheduled").Include(
                "~/client/scripts/app/payments.scheduled.v1.js"));

            BundleTable.EnableOptimizations = false;

        }
    }
}