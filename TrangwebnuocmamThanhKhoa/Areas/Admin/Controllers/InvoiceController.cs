using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;
using TrangwebnuocmamThanhKhoa.Areas.Admin.Data;
using CKFinder.Settings;
using System.Threading.Tasks;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    public class InvoiceController : BaseController
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        // GET: Admin/Invoice
        public ActionResult InvoiceList()
        {
            var invoices = db.Invoices
                .OrderBy(invoice => invoice.Status == 0 ? 1 :
                                    invoice.Status == 1 ? 2 :
                                    invoice.Status == -1 ? 3 : 4)
                .ThenByDescending(invoice => invoice.CreateAt)
                .ToList();

            ViewBag.CountInvoice = invoices.Count();
            return View(invoices);
        }

        public ActionResult InvoiceDetail(int id)
        {
            var invoice = db.Invoices.FirstOrDefault(inv => inv.Id == id);

            if (invoice == null)
            {
                return HttpNotFound();
            }

            UpdatePaymentMethod(id);

            var invoiceData = new Invoice_Data
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.CustomerName,
                CustomerPhone = invoice.CustomerPhone,
                CustomerAddress = invoice.CustomerAddress,
                Status = (int)invoice.Status,
                CreateAt = invoice.CreateAt ?? DateTime.MinValue,
                PaymentMethods = invoice.PaymentMethods ?? 0,
                StaffId = invoice.StaffId ?? 0,
                Code = invoice.Code,
                TotalAmount = invoice.TotalAmount ?? 0
            };


            var invoicedetails = db.InvoiceDetails.Where(detail => detail.InvoiceId == id).ToList();

            foreach (var detail in invoicedetails)
            {
                var product = db.Products.FirstOrDefault(p => p.Id == detail.ProductId);

                if (product != null)
                {
                    var invoiceDetail = new Invoice_Detail
                    {
                        InvoiceId = detail.InvoiceId,
                        ProductId = detail.ProductId,
                        ProductName = product.Name,
                        ProductImages = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                        ProductList = detail.ProductList,
                        Quantity = (int)detail.Quantity,
                        TotalPrice = (int)detail.TotalPrice
                    };

                    invoiceData.InvoiceDetails.Add(invoiceDetail);
                }
            }

            return View(invoiceData);
        }

        public void UpdatePaymentMethod(int orderId)
        {
            var order = db.Invoices.SingleOrDefault(o => o.Id == orderId);

            if (order != null)
            {
                order.PaymentMethods = 1;

                db.SubmitChanges();
            }
        }


        [HttpPost]
        public JsonResult ConfirmInvoice(int orderId)
        {
            var order = db.Invoices.SingleOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = 1;
                db.SubmitChanges();
                return Json(new { success = true, message = "Đơn hàng đã được xác nhận thành công!" });
            }
            return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
        }

        [HttpPost]
        public JsonResult CancelOrder(int orderId)
        {
            var order = db.Invoices.SingleOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = -1;
                db.SubmitChanges();
                return Json(new { success = true, message = "Đã hủy đơn hàng!" });
            }
            return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
        }

        [HttpGet]
        public JsonResult GetInvoicesWithPaymentMethodZero()
        {
            var invoices = db.Invoices
                .Where(invoice => invoice.PaymentMethods == 0)
                .ToList();
            
            return Json(invoices, JsonRequestBehavior.AllowGet);
        }
    }
}