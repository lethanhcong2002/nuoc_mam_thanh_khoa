using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set;}
        public string Summary { get; set;}
        public string CoverImage { get; set;}
        public string Content { get; set;}
        public int Views { get; set;}
        public string MetaTitle { get; set;}
        public string CreateAt { get; set;}
    }
}