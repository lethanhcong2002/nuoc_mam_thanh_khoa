using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Areas.Admin.Data;
using TrangwebnuocmamThanhKhoa.Models;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "nmtk-admin")]
    [RoutePrefix("master-admin")]
    [Route("{action}")]
    public class AccountUserController : BaseController
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/AccountUser

        [Route("quan-ly-user")]
        public ActionResult ListUserAccount()
        {
            var role = db.Users.Where(n => n.Status == 1).ToList();
            ViewBag.CountUser = role.Count;
            return View(role);
        }

        [Route("chi-tiet-user")]
        public ActionResult DetailsUser(int id)
        {
            try
            {
                var detals = db.Users.Where(n => n.Id == id).SingleOrDefault();
                return View(detals);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while retrieving the record: " + ex.Message);
                return View();
            }
        }

        [Route("cap-nhat-thong-tin-user")]
        public ActionResult EditUser(int id)
        {
            var edituser = db.Users.Where(n => n.Id == id).SingleOrDefault();
            return View(edituser);
        }
        [HttpPost]
        [Route("cap-nhat-thong-tin-user")]
        public ActionResult EditUser(int id,FormCollection f)
        {
            try
            {
                var edituser = db.Users.Where(n => n.Id == id).SingleOrDefault();
                edituser.Name = f["Name"];
                edituser.Phone = f["Phone"];
                edituser.Email = f["Email"];
                edituser.Address = f["Address"];
                edituser.CreateAt = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("DetailsUser", new { id = edituser.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Route("xoa-thong-tin-user")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                var deleteuser = db.Users.Where(n => n.Id == id).SingleOrDefault();
                    deleteuser.Status = 0;
                    db.SubmitChanges();
                    return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}