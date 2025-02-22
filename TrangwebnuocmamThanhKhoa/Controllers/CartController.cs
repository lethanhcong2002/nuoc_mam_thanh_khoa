using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Diagnostics;

namespace TrangwebnuocmamThanhKhoa.Controllers
{
    public class CartController : Controller
    {
        private readonly DataClasses1DataContext _dbContext = new DataClasses1DataContext();

        [HttpPost]
        public JsonResult AddToCart(int ProductId, string PLName, int Quantity, int CustomerId)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == CustomerId);

            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại." });
            }

            try
            {
                var existingCartItem = _dbContext.Carts
                    .FirstOrDefault(c => c.ProductID == ProductId && c.PLName == PLName && c.CustomerID == CustomerId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += Quantity;
                    _dbContext.SubmitChanges();
                }
                else
                {
                    var cartItem = new Cart
                    {
                        ProductID = ProductId,
                        PLName = PLName,
                        Quantity = Quantity,
                        CustomerID = CustomerId
                    };

                    _dbContext.Carts.InsertOnSubmit(cartItem);
                    _dbContext.SubmitChanges();
                }

                return Json(new { success = true, message = "Thêm vào giỏ hàng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }


        [HttpGet]
        public JsonResult GetCartItems(int userId)
        {
            var cartItems = _dbContext.Carts
                .Where(c => c.CustomerID == userId)
                .ToList();

            var productIds = cartItems.Select(item => item.ProductID).Distinct().ToList();

            var products = _dbContext.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionary(p => p.Id, p => p);

            var detailedCartItems = cartItems.Select(item =>
            {
                products.TryGetValue(item.ProductID, out var product);
                int productPrice = 0;

                if (product != null && !string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);
                    if (productItems != null && productItems.Any())
                    {
                        productPrice = (int)productItems.FirstOrDefault().Gia;
                    }
                }

                return new CartDetail
                {
                    ProductId = item.ProductID,
                    ProductName = product?.Name,
                    PLName = item.PLName,
                    ProductPrice = productPrice,
                    Quantity = (int)item.Quantity,
                    CustomerId = item.CustomerID
                };
            }).ToList();

            return Json(detailedCartItems, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateCartItem(int userId, int productId, string plName, string action)
        {
            var cartItem = _dbContext.Carts
                .FirstOrDefault(c => c.CustomerID == userId && c.ProductID == productId && c.PLName == plName);

            if (cartItem != null)
            {
                if (action == "increment")
                {
                    cartItem.Quantity++;
                }
                else if (action == "decrement")
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                    }
                }
                else if (action == "delete")
                {
                    _dbContext.Carts.DeleteOnSubmit(cartItem);
                }

                _dbContext.SubmitChanges();
            }

            return GetCartItems(userId);
        }

        [HttpPost]
        public JsonResult SubmitOrder(FormCollection form)
        {
            try
            {
                var user = Session["User"] as User;
                var customerId = user != null ? user.Id.ToString() : null;
                var fullname = form["Fullname"];
                var phone = form["Phone"];
                var address = form["Address"];

                // Tạo mã hóa đơn
                var orderCode = GenerateOrderCode();

                // Tạo một đối tượng Invoice
                var invoice = new Invoice
                {
                    Code = orderCode,
                    CustomerId = customerId != null ? int.Parse(customerId) : (int?)null,
                    CustomerName = fullname,
                    CustomerPhone = phone,
                    CustomerAddress = address,
                    PaymentMethods = 0,
                    Status = 0,
                    CreateAt = DateTime.Now
                };

                // Lưu Invoice vào cơ sở dữ liệu
                _dbContext.Invoices.InsertOnSubmit(invoice);
                _dbContext.SubmitChanges();

                // Lấy InvoiceId của hóa đơn đã tạo
                int invoiceId = invoice.Id;
                double totalAmount = 0;

                // Thu thập và lưu các chi tiết hóa đơn
                var cartItems = new List<InvoiceDetail>();
                int index = 0;
                while (form[$"CartItems[{index}].ProductId"] != null)
                {
                    var productId = int.Parse(form[$"CartItems[{index}].ProductId"]);
                    var plName = form[$"CartItems[{index}].PLName"];
                    var quantity = int.Parse(form[$"CartItems[{index}].Quantity"]);
                    var productPrice = float.Parse(form[$"CartItems[{index}].ProductPrice"]);

                    var cartItem = new InvoiceDetail
                    {
                        ProductId = productId,
                        ProductList = plName,
                        Quantity = quantity,
                        TotalPrice = productPrice * quantity,
                        InvoiceId = invoiceId
                    };
                    cartItems.Add(cartItem);
                    totalAmount += (int)cartItem.TotalPrice;
                    index++;
                }

                foreach (var item in cartItems)
                {
                    _dbContext.InvoiceDetails.InsertOnSubmit(item);
                }
                invoice.TotalAmount = totalAmount;
                _dbContext.SubmitChanges();

                if (customerId != null)
                {
                    var cartItemsToDelete = _dbContext.Carts.Where(c => c.CustomerID == int.Parse(customerId));
                    _dbContext.Carts.DeleteAllOnSubmit(cartItemsToDelete);
                    _dbContext.SubmitChanges();
                }

                SendInvoiceEmail(invoice);

                return Json(new { success = true, message = "Đơn hàng đã được lưu thành công!", orderCode });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        private string GenerateOrderCode()
        {
            string newCode;
            Random random = new Random();
            bool isDuplicate;

            do
            {
                newCode = random.Next(0, 99999999).ToString("D8");

                isDuplicate = _dbContext.Invoices.Any(i => i.Code == newCode);

            } while (isDuplicate);

            return newCode;
        }

        private string GetProductNameById(int productId)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == productId);

            return product != null ? product.Name : "Sản phẩm không tìm thấy";
        }

        private void SendInvoiceEmail(Invoice invoice)
        {
            string subject = $"XÁC NHẬN ĐƠN ĐẶT HÀNG MỚI {invoice.Code}";
            string body = $@"
            <html>
            <body>
                <h2>Xin chào,</h2>
                <p>Có đơn hàng mới mã số <strong>{invoice.Code}</strong>, ngày <strong>{invoice.CreateAt:dd/MM/yyyy}</strong>.</p>
                <p>Thông tin chi tiết đơn hàng như sau:</p>
                <table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse; width:100%;'>
                    <tr>
                        <th style='text-align:left;'>Mã hóa đơn:</th>
                        <td>{invoice.Code}</td>
                    </tr>
                    <tr>
                        <th style='text-align:left;'>Khách hàng:</th>
                        <td>{invoice.CustomerName}</td>
                    </tr>
                    <tr>
                        <th style='text-align:left;'>Số điện thoại:</th>
                        <td>{invoice.CustomerPhone}</td>
                    </tr>
                    <tr>
                        <th style='text-align:left;'>Địa chỉ:</th>
                        <td>{invoice.CustomerAddress}</td>
                    </tr>
                    <tr>
                        <th style='text-align:left;'>Ngày đặt hàng:</th>
                        <td>{invoice.CreateAt:dd/MM/yyyy}</td>
                    </tr>
                </table>

                <h3>Chi tiết sản phẩm đặt hàng:</h3>
                <table border='1' cellpadding='5' cellspacing='0' style='border-collapse:collapse; width:100%;'>
                    <thead>
                        <tr>
                            <th>Tên sản phẩm</th>
                            <th>Số lượng</th>
                            <th>Đơn giá</th>
                            <th>Thành tiền</th>
                        </tr>
                    </thead>
                    <tbody>";

            var invoiceDetails = _dbContext.InvoiceDetails.Where(d => d.InvoiceId == invoice.Id).ToList();
            foreach (var item in invoiceDetails)
            {
                string productName = GetProductNameById(item.ProductId);
                body += $@"
                        <tr>
                            <td>{productName}</td>
                            <td>{item.Quantity}</td>
                            <td>{(item.TotalPrice / item.Quantity)}</td>
                            <td>{item.TotalPrice}</td>
                        </tr>";
            }

            body += $@"
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colspan='3' style='text-align:right;'><strong>Tổng thành tiền:</strong></td>
                            <td><strong>{invoice.TotalAmount}</strong></td>
                        </tr>
                    </tfoot>
                </table>
        
                <p>Vui lòng xác nhận đơn hàng!</p>
                <p><em>(Có file đính kèm chi tiết đặt hàng)</em></p>
                <p>Trân trọng!</p>
            </body>
            </html>";

            string recipientEmail = "";
            string senderEmail = "";
            string senderPassword = "";

            try
            {
                var stream = new MemoryStream();
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Invoice");
                    worksheet.Cells["A1"].Value = "HÓA ĐƠN BÁN HÀNG";
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["A1:E1"].Merge = true;

                    worksheet.Cells["A3"].Value = $"Số hóa đơn: {invoice.Code}";
                    worksheet.Cells["A4"].Value = $"Ngày: {invoice.CreateAt:dd/MM/yyyy}";
                    worksheet.Cells["A5"].Value = $"Khách hàng: {invoice.CustomerName}";
                    worksheet.Cells["A6"].Value = $"Địa chỉ: {invoice.CustomerAddress}";
                    worksheet.Cells["A7"].Value = $"Số điện thoại: {invoice.CustomerPhone}";

                    worksheet.Cells["A9"].Value = "Tên sản phẩm";
                    worksheet.Cells["B9"].Value = "Loại";
                    worksheet.Cells["C9"].Value = "Số lượng";
                    worksheet.Cells["D9"].Value = "Đơn giá";
                    worksheet.Cells["E9"].Value = "Thành tiền";
                    worksheet.Cells["A9:E9"].Style.Font.Bold = true;
                    worksheet.Cells["A9:E9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    int row = 10;
                    decimal totalAmount = 0;
                    foreach (var item in invoiceDetails)
                    {
                        string productName = GetProductNameById(item.ProductId);
                        worksheet.Cells[row, 1].Value = productName;
                        worksheet.Cells[row, 2].Value = item.ProductList;
                        worksheet.Cells[row, 3].Value = item.Quantity;
                        worksheet.Cells[row, 4].Value = item.TotalPrice / item.Quantity;
                        worksheet.Cells[row, 5].Value = item.TotalPrice;

                        totalAmount += (decimal)item.TotalPrice;
                        row++;
                    }

                    worksheet.Cells[row, 4].Value = "Tổng cộng";
                    worksheet.Cells[row, 5].Value = totalAmount;
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Style.Font.Bold = true;

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    package.Save();
                }
                stream.Position = 0;
                string fileName = "Hóa đơn đặt hàng.xlsx";

                // Gửi email kèm file Excel
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(recipientEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    mail.Attachments.Add(new Attachment(stream, fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
                Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }

    }
}
