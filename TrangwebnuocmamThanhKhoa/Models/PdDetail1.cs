using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Models
{
    public class PdDetail1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Images { get; set; }
        public int Price { get; set; }
        public string MetaTitle { get; set; }
        public List<ProductListDetail> ProductListDetails { get; set; }
    }
}