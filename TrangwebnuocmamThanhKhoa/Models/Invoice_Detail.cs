using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Models
{
    public class Invoice_Detail
    {
        public int InvoiceId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImages { get; set; }
        public string ProductList { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}