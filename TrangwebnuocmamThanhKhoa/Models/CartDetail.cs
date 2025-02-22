using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Models
{
    public class CartDetail
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string PLName { get; set; }
        public int ProductPrice { get; set; }
        public int Quantity { get; set; }

        public int CustomerId { get; set; }
    }

}