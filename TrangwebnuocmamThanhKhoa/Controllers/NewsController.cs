using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;

namespace WebApplication2.Controllers
{
    public class NewsController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: News
        public ActionResult MainNews()
        {
            var news = GetAllNews();
            return View(news);
        }
        private List<News> GetAllNews()
        {
            var news = db.News.OrderByDescending(n => n.CreateAt).ToList();
            var newList = new List<News>();

            foreach (var new_data in news)
            {
                var new_datadetail = new News
                {
                    Id = new_data.Id,
                    Title = new_data.Title,
                    Summary = new_data.Summary,
                    Views = (int)new_data.Views,
                    Content = new_data.Content,
                    CoverImage = new_data.CoverImage,
                    CreateAt = new_data.CreateAt.ToString(),
                    MetaTitle = new_data.MetaTitle,
                };


                newList.Add(new_datadetail);

            }
            return newList;
        }
        public ActionResult NewsDetail(int id, string metatitle)
        {
            IncreaseNewsViews(id);
            var detail = db.News.FirstOrDefault(x => x.Id == id);
            var new_datadetail = new News
            {
                Id = detail.Id,
                Title = detail.Title,
                Summary = detail.Summary,
                Views = (int)detail.Views,
                Content = detail.Content,
                CoverImage = detail.CoverImage,
                CreateAt = detail.CreateAt.ToString(),
                MetaTitle = detail.MetaTitle,
            };
            return View(new_datadetail);
        }
        public PartialViewResult NewsTop()
        {
            var topNews = GetTopNews();
            return PartialView("NewsTop", topNews);
        }

        private List<News> GetTopNews()
        {
            var topNews = db.News.OrderByDescending(x => x.Views).Take(3).ToList();
            var newList = new List<News>();
            foreach (var new_data in topNews)
            {
                var new_datadetail = new News
                {
                    Id = new_data.Id,
                    Title = new_data.Title,
                    Summary = new_data.Summary,
                    Views = (int)new_data.Views,
                    Content = new_data.Content,
                    CoverImage = new_data.CoverImage,
                    CreateAt = new_data.CreateAt.ToString(),
                    MetaTitle = new_data.MetaTitle,
                };


                newList.Add(new_datadetail);

            }
            return newList;
        }

        public PartialViewResult NewsCorner()
        {
            var topNews = GetCornerNews();
            return PartialView("NewsCorner", topNews);
        }

        private List<News> GetCornerNews()
        {
            var topNews = db.News
                .Where(x => x.Status == true)
                .OrderByDescending(x => x.CreateAt)
                .Take(3)
                .ToList();

            var newList = new List<News>();

            foreach (var new_data in topNews)
            {
                var new_datadetail = new News
                {
                    Id = new_data.Id,
                    Title = new_data.Title,
                    Summary = new_data.Summary,
                    Views = (int)new_data.Views,
                    Content = new_data.Content,
                    CoverImage = new_data.CoverImage,
                    CreateAt = new_data.CreateAt.ToString(),
                };

                newList.Add(new_datadetail);
            }

            return newList;
        }

        private void IncreaseNewsViews(int newsId)
        {
            var news = db.News.FirstOrDefault(x => x.Id == newsId);

            if (news != null)
            {
                news.Views++;
                db.SubmitChanges();
            }
        }
    }
}