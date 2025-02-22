using static Unidecode.NET.Unidecoder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;
using TrangwebnuocmamThanhKhoa.Areas.Admin.Data;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    public class NewController : BaseController
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public ActionResult ListNew()
        {
            var listnew = db.News.OrderByDescending(n => n.CreateAt).ToList();
            ViewBag.CountNew = listnew.Count;
            return View(listnew);
        }

        public ActionResult CreateNew()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateNew(FormCollection f)
        {
            try
            {
                New tintuc = new New();
                tintuc.Title = f["Title"];
                tintuc.Summary = f["Summary"];
                tintuc.Content = f["NoiDung"];
                tintuc.CoverImage = f["HinhAnhThumbNail"];
                tintuc.Views = 0;
                tintuc.MetaTitle = f["Title"].Unidecode().Replace(" ", "-") +"-"+DateTime.Now.ToString("MMddyyHHmm");
                tintuc.Status = false;
                tintuc.CreateAt = DateTime.Now;
                tintuc.UpdateAt = DateTime.Now;
                db.News.InsertOnSubmit(tintuc);
                db.SubmitChanges();
                return RedirectToAction("ListNew");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult EditNew(int id)
        {
            var editnew = db.News.SingleOrDefault(n => n.Id == id);
            return View(editnew);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditNew(int id , FormCollection f)
        {
            try
            {
                New tintuc = db.News.SingleOrDefault(n => n.Id == id);
                tintuc.Title = f["Title"];
                tintuc.Summary = f["Summary"];
                tintuc.Content = f["NoiDung"];
                tintuc.CoverImage = f["HinhAnhThumbNail"];
                tintuc.Views = int.Parse(f["Views"]);
                tintuc.MetaTitle = f["Title"].Unidecode().Replace(" ", "-") + "-" + DateTime.Now.ToString("MMddyyHHmm");
                tintuc.UpdateAt = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("ListNew");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }

        public ActionResult DeleteNew(int id)
        {
            try
            {
                var deletenew = db.News.SingleOrDefault(n => n.Id == id);
                db.News.DeleteOnSubmit(deletenew);
                db.SubmitChanges();
                return RedirectToAction("ListNew");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the record: " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, bool isChecked)
        {
            try
            {
                var check = db.News.SingleOrDefault(m => m.Id == id);
                if (isChecked == true)
                {
                    check.Status = true;
                }
                else
                {
                    check.Status = false;
                }
                db.SubmitChanges();
                return Json(new { success = true, message = "Chuyển trạng thái thành công" });
            }
            catch (Exception)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái của tin tức." });
            }
        }
    }
}