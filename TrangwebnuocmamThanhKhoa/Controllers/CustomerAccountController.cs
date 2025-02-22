using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TrangwebnuocmamThanhKhoa.Models;
using Unidecode.NET;

namespace WebApplication2.Controllers
{
    public class CustomerAccountController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
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

        public ActionResult RegisterCustomer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterCustomer(
            string Name,
            string Phone,
            string Address,
            string Password,
            string ConfirmPassword,
            string Province,
            string District,
            string Ward,
            string ProvinceText,
            string DistrictText,
            string WardText)
        {
            var existingUser = db.Users.SingleOrDefault(u => u.Phone == Phone);
            if (existingUser != null)
            {
                return View();
            }

            User newUser = new User
            {
                Name = Name,
                Phone = Phone,
                Address = Address + ", " + WardText + ", " + DistrictText + ", " + ProvinceText,
                Password = GetMd5Hash(Password),
                Role = 1,
                CreateAt = DateTime.Now,
                Status = 1,
                MetaTitle = Name.Unidecode().Replace(" ", "-").ToLower()
            };

            db.Users.InsertOnSubmit(newUser);
            db.SubmitChanges();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ForgetPassword(string Phone, string Password, string RePassword)
        {
            if (Password != RePassword)
            {
                return Json(new { success = false, message = "Mật khẩu và xác nhận mật khẩu không khớp." });
            }

            var existingUser = db.Users.SingleOrDefault(u => u.Phone == Phone);
            if (existingUser == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            existingUser.Password = GetMd5Hash(Password);
            db.SubmitChanges();

            return Json(new { success = true, message = "Mật khẩu đã được thay đổi thành công." });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Phone, string Password)
        {
            var user = db.Users.SingleOrDefault(a => a.Phone == Phone && a.Password == GetMd5Hash(Password) && a.Role == 1);

            if (user != null)
            {
                Session["User"] = user;
                FormsAuthentication.SetAuthCookie(user.Name, false);
                return RedirectToAction("Index", "Home");
            }

            TempData["LoginError"] = "Tên đăng nhập hoặc mật khẩu không chính xác.";

            string returnUrl = Request.UrlReferrer?.ToString();

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult DetailInforCustomer(int id, string metatitle)
        {
            var detail = db.Users.FirstOrDefault(x => x.Id == id);
            var Infordetail = new UserAccount
            {
                Id = detail.Id,
                Name = detail.Name,
                Phone = detail.Phone,
                Email = detail.Email,
                Address = detail.Address,
                MetaTitle = metatitle,
            };

            return View(Infordetail);
        }

        [HttpPost]
        public JsonResult SaveChanges(int id, string name, string address, string email, string phone)
        {
            var customer = db.Users.FirstOrDefault(n => n.Id == id);
            if (customer != null)
            {
                customer.Name = name;
                customer.Address = address;
                customer.Email = email;
                customer.Phone = phone;

                db.SubmitChanges();
                return Json(new { success = true, message = "Thông tin đã được cập nhật thành công." });
            }

            return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật thông tin." });
        }
    }
}