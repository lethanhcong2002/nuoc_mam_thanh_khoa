using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!User.Identity.IsAuthenticated)
            {
                filterContext.Result = RedirectToAction("ViewLogin", "LoginAccount");
            }
            else
            {
                var admin = Session["Admin"];
                if (admin != null)
                {
                    ViewBag.Admin = admin;
                }
            }
        }
    }
}