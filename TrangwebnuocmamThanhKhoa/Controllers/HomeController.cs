using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace TrangwebnuocmamThanhKhoa.Controllers
{
    public class HomeController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public ActionResult Index()
        {
            var products = GetAllProducts();
            ViewBag.User = Session["User"];
            return View(products);
        }

        private List<PDDetail> GetAllProducts()
        {
            var products = db.Products
                     .Where(p => p.Status == true)
                     .OrderByDescending(p => p.CreateAt)
                     .ToList();

            var productDetail = new List<PDDetail>();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle,
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }
            return productDetail.Take(8).ToList();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {      
            return View();
        }

        [HttpPost]
        public JsonResult SendEmail(string name, string phone, string email, string address, string message)
        {
            string currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            string subject = "THƯ YÊU CẦU HỖ TRỢ TỪ KHÁCH HÀNG";
            string body = $@"
                <html>
                <head>
                    <style>
                        .email-body {{ font-family: Arial, sans-serif; }}
                        .email-header {{ font-size: 24px; font-weight: bold; }}
                        .email-content {{ margin-top: 20px; }}
                        .email-content p {{ margin: 5px 0; }}
                    </style>
                </head>
                <body>
                    <div class='email-body'>
                        <div class='email-header'>THƯ YÊU CẦU HỖ TRỢ TỪ KHÁCH HÀNG</div>
                        <div class='email-content'>
                            <p><strong>Xin chào,</strong></p>
                            <p>Có khách hàng liên hệ hỗ trợ ngày {currentDate}.</p>
                            <p><strong>Tên khách hàng:</strong> {name}</p>
                            <p><strong>Số điện thoại:</strong> {phone}</p>
                            <p><strong>Địa chỉ:</strong> {address}</p>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Nội dung hỗ trợ như sau:</strong><br>{message}</p>
                            <p>Vui lòng xác nhận hỗ trợ!</p>
                            <p>Trân trọng!</p>
                        </div>
                    </div>
                </body>
                </html>";
            string recipientEmail = "";
            string senderEmail = "";
            string senderPassword = "";
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }

                return Json(new { success = true, message = "Email đã được gửi thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gửi email thất bại: " + ex.Message });
            }
        }

        public ActionResult ShoppingCart()
        {
            return View();
        }

        public ActionResult Notification()
        {
            return View();  
        }

        public ActionResult Rules()
        {
            return View();
        }
    }
}