using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using System.Web.Routing;



namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Data
{
    public static class HtmlHelpers
    {
        public static string IsActive(this HtmlHelper html, string controllers = null, string actions = null, string cssClass = "active")
        {
            ViewContext viewContext = html.ViewContext;
            RouteData routeData = viewContext.RouteData;

            string currentAction = routeData.GetRequiredString("action");
            string currentController = routeData.GetRequiredString("controller");

            string[] acceptedActions = (actions ?? currentAction).Split(',');
            string[] acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
                cssClass : string.Empty;
        }
    }
}