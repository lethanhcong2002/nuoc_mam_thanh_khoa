using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TrangwebnuocmamThanhKhoa.Controllers
{
    public class PartialController : Controller
    {
        // GET: Partial
        public PartialViewResult NavbarUser()
        {
            ViewBag.User = Session["User"];
            return PartialView();
        }
    }
}