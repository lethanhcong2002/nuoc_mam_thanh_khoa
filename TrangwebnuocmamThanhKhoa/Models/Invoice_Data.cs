using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Models
{
    public class Invoice_Data
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public int PaymentMethods { get; set; }
        public int StaffId { get; set; }
        public string Code { get; set; }
        public double TotalAmount { get; set; }
        public List<Invoice_Detail> InvoiceDetails { get; set; } = new List<Invoice_Detail>();

    }
}