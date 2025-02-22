using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Data
{
    public class BreadcrumbItem
    {
        public string NamePage { get; set; }
        public string Url { get; set; }
        public bool IsActive => string.IsNullOrEmpty(Url);
    }
}