using System.Web;
using System.Web.Optimization;

namespace Almacen.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // App Styles
            bundles.Add(new StyleBundle("~/Content/appCss").Include(
                "~/Content/app/css/app.css",
                "~/Content/toastr.css"
            ));

            // Bootstrap Styles
            bundles.Add(new StyleBundle("~/Content/bootstrapCss").Include(
                "~/Content/app/css/bootstrap.css", new CssRewriteUrlTransform()
            ));


            bundles.Add(new ScriptBundle("~/bundles/Angle").Include(
                // App init
                "~/Scripts/app/app.init.js",
                // Modules
                "~/Scripts/app/modules/bootstrap-start.js",
                "~/Scripts/app/modules/clear-storage.js",
                "~/Scripts/app/modules/constants.js",
                "~/Scripts/app/modules/localize.js",
                "~/Scripts/app/modules/navbar-search.js",
                "~/Scripts/app/modules/panel-tools.js",
                "~/Scripts/app/modules/play-animation.js",
                "~/Scripts/app/modules/sidebar.js",
                "~/Scripts/app/modules/toggle-state.js",
                "~/Scripts/app/modules/utils.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
              "~/Vendor/datatables/media/js/jquery.dataTables.min.js",
              "~/Vendor/datatables-colvis/js/dataTables.colVis.js",
              "~/Vendor/datatable-bootstrap/js/dataTables.bootstrap.js",
              "~/Vendor/datatable-bootstrap/js/dataTables.bootstrapPagination.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/datatablesCss")
              .Include("~/Vendor/datatables-colvis/css/dataTables.colVis.css", new CssRewriteUrlTransform())
              .Include("~/Vendor/datatable-bootstrap/css/dataTables.bootstrap.css", new CssRewriteUrlTransform())
            );
            // Main Vendor


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-2.1.3.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/toastr").Include(
                        "~/Scripts/toastr.js",
                        "~/Scripts/toastrOptions.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            // Vendor Plugins

            bundles.Add(new ScriptBundle("~/bundles/filestyle").Include(
              "~/Vendor/bootstrap-filestyle/src/bootstrap-filestyle.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/animo").Include(
              "~/Vendor/animo.js/animo.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/chosen").Include(
              "~/Vendor/chosen_v1.8.5/chosen.jquery.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/chosenCss").Include(
              "~/Vendor/chosen_v1.8.5/chosen.css", new CssRewriteUrlTransform()
            ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
              "~/Scripts/moment.js",
              "~/Scripts/moment-with-locales.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datetimepicker").Include(
              "~/Scripts/bootstrap-datetimepicker.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/datetimepickerCss").Include(
              "~/Content/bootstrap-datetimepicker.css", new CssRewriteUrlTransform()
            ));

            bundles.Add(new StyleBundle("~/bundles/Spinkit").Include(
                "~/Vendor/spinkit/css/spinkit.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/LoadersCss").Include(
                "~/Vendor/loaders.css/loaders.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/simpleLineIcons").Include(
              "~/Vendor/simple-line-icons/css/simple-line-icons.css", new CssRewriteUrlTransform()
            ));


            bundles.Add(new ScriptBundle("~/bundles/storage").Include(
              "~/Vendor/jQuery-Storage-API/jquery.storageapi.js"
            ));

            bundles.Add(new StyleBundle("~/bundles/whirl").Include(
              "~/Vendor/whirl/dist/whirl.css"
            ));

            bundles.Add(new StyleBundle("~/bundles/fontawesome").Include(
              "~/Vendor/fontawesome/css/font-awesome.min.css", new CssRewriteUrlTransform()
            ));

            bundles.Add(new StyleBundle("~/bundles/animatecss").Include(
              "~/Vendor/animate.css/animate.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/localize").Include(
              "~/Vendor/jquery-localize-i18n/dist/jquery.localize.js"
            ));

        }
    }
}
