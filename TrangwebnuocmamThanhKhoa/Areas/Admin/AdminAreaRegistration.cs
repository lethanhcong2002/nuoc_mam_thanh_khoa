using System.Web.Mvc;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            //--------Login
            context.MapRoute(
                "Admin_Login",
                "admin",
                new { controller = "LoginAccount", action = "ViewLogin", id = UrlParameter.Optional }
            );
            //--------Account
            context.MapRoute(
                "Admin_Account_Manage",
                "admin/quan-ly-tai-khoan",
                new { controller = "AccountUser", action = "ListUserAccount", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_Account_Info",
                "admin/thong-tin-tai-khoan/{metatitle}-{id}",
                new { controller = "AccountUser", action = "DetailsUser", id = UrlParameter.Optional }
            );
            context.MapRoute(
                "Admin_Account_Update",
                "admin/cap-nhat-thong-tin-tai-khoan",
                new { controller = "AccountUser", action = "EditUser" }
            );
            //--------Product
            context.MapRoute(
                "Admin_Category_Manage",
                "admin/danh-sach-loai",
                new { controller = "Product", action = "ListCategory" }
            );
            context.MapRoute(
                "Admin_Category_New",
                "admin/danh-sach-loai/them-moi",
                new { controller = "Product", action = "CreateCategory" }
            );
            context.MapRoute(
                "Admin_Product_List",
                "admin/danh-sach-san-pham",
                new { controller = "Product", action = "EditCategory" }
            );
            context.MapRoute(
                "Admin_Product_New",
                "admin/danh-sach-san-pham/them-moi",
                new { controller = "Product", action = "CreateProduct" }
            );
            context.MapRoute(
                "Admin_Product_Update",
                "admin/danh-sach-san-pham/cap-nhat",
                new { controller = "Product", action = "EditProduct" }
            );
            //--------News
            context.MapRoute(
                "Admin_News",
                "admin/danh-sach-tin-tuc",
                new { controller = "New", action = "ListNew" }
            );
            context.MapRoute(
                "Admin_News_Add",
                "admin/danh-sach-tin-tuc/them-moi",
                new { controller = "New", action = "CreateNew" }
            );
            context.MapRoute(
                "Admin_News_Update",
                "admin/danh-sach-tin-tuc/cap-nhat",
                new { controller = "New", action = "EditNew" }
            );
            //--------Invoice
            context.MapRoute(
                "Admin_Invoice",
                "admin/danh-sach-don-hang",
                new { controller = "Invoice", action = "InvoiceList" }
            );
            context.MapRoute(
                "Admin_Invoice_Detail",
                "admin/danh-sach-don-hang/chi-tiet",
                new { controller = "Invoice", action = "InvoiceDetail" }
            );
            //--------Default
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}