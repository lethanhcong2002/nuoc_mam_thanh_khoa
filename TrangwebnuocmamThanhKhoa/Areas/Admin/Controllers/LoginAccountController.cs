using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TrangwebnuocmamThanhKhoa.Models;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    public class LoginAccountController : Controller
    {
        protected DataClasses1DataContext db = new DataClasses1DataContext();
        static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }


        public ActionResult ViewLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ViewLogin(string Account, string Password)
        {
            var admin = db.Users.SingleOrDefault(a => a.Account == Account && a.Password == GetMd5Hash(Password) && a.Role == 0);
            Debug.WriteLine(GetMd5Hash(Password));
            if (admin != null)
            {
                Session["Admin"] = admin;
                FormsAuthentication.SetAuthCookie(admin.Name, false);
                return RedirectToAction("ListUserAccount", "AccountUser");

            }
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session["Admin"] = null;

            FormsAuthentication.SignOut();

            return RedirectToAction("ViewLogin", "LoginAccount");
        }

    }
}