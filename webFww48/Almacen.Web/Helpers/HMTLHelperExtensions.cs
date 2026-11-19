using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Almacen.Web
{
    public static class HTMLHelperExtensions
    {
        public static string isActive(this HtmlHelper html, string controller = null, string action = null)
        {
            string activeClass = "active"; // change here if you another name to activate sidebar items
            // detect current app state
            string actualAction = (string) html.ViewContext.RouteData.Values["action"];
            string actualController = (string) html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = actualController;

            if (String.IsNullOrEmpty(action))
                action = actualAction;

            return (controller == actualController && action == actualAction) ? activeClass : String.Empty;
        }

        public static MvcHtmlString ColorPicker(this HtmlHelper helper, string value, string controlType = "saturation", string cssName= "form-control",string scriptPathPicker="")
        {

            string script = "";
            if (!string.IsNullOrEmpty(scriptPathPicker))
            {
                var builder = new TagBuilder("script");
                builder.MergeAttribute("src", script);
                builder.MergeAttribute("type", "text/javascript");
                MvcHtmlString strScript = MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
            }

            var tagBuilder = new TagBuilder("input");
            tagBuilder.AddCssClass(cssName);
            tagBuilder.Attributes.Add("data-control", controlType.ToString());
            tagBuilder.InnerHtml = value;
            return new MvcHtmlString(tagBuilder.ToString());

        }

    }
}
