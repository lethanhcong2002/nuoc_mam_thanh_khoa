using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrangwebnuocmamThanhKhoa.Code
{
    public static class HtmlExtensions
    {
        public static string IsActive(this HtmlHelper html, string controllers = null, string actions = null, string cssClass = "active")
        {
            var routeData = html.ViewContext.RouteData;

            string currentAction = routeData.GetRequiredString("action");
            string currentController = routeData.GetRequiredString("controller");

            string[] acceptedActions = (actions ?? currentAction).Split(',');
            string[] acceptedControllers = (controllers ?? currentController).Split(',');

            // Check if any child actions match
            foreach (var action in acceptedActions)
            {
                if (routeData.Values["action"].ToString().Equals(action.Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    return cssClass;
                }
            }

            // Check if parent action and controller match
            if (acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController))
            {
                return cssClass;
            }

            return string.Empty;
        }
    }
}