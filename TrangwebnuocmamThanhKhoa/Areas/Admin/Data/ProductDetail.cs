using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Data
{
    public class ProductDetail
    {
        public string Name { get; set; }
        public int Thetich { get; set; }
        public int Gia { get; set; }
        public bool Status { get; set; }
    }
}