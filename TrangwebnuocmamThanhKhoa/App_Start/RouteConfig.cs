using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TrangwebnuocmamThanhKhoa
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Register",
                url: "tao-tai-khoan",
                defaults: new { controller = "CustomerAccount", action = "RegisterCustomer", id = UrlParameter.Optional }
            );

            //------Account
            routes.MapRoute(
                name: "Account Detail",
                url: "khach-hang/{metatitle}-{id}",
                defaults: new { controller = "CustomerAccount", action = "DetailInforCustomer" }
            );
            //------Home
            routes.MapRoute(
                name: "About",
                url: "cau-chuyen-ve-Thanh-Khoa",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Contact",
                url: "lien-he",
                defaults: new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "ShoppingCart",
                url: "gio-hang",
                defaults: new { controller = "Home", action = "ShoppingCart", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Rules",
                url: "chinh-sach-xu-ly-khieu-nai",
                defaults: new { controller = "Home", action = "Rules", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Notification",
                url: "thong-bao",
                defaults: new { controller = "Home", action = "Notification", id = UrlParameter.Optional }
            );
            //------News
            routes.MapRoute(
                name: "New",
                url: "tin-tuc",
                defaults: new { controller = "News", action = "MainNews", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "New Detail",
                url: "tin-tuc/{metatitle}-{id}",
                defaults: new { controller = "News", action = "NewsDetail", id = UrlParameter.Optional }
            );

            //------Products
            routes.MapRoute(
                name: "Product",
                url: "san-pham",
                defaults: new { controller = "Products", action = "ProductList", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Product Detail",
                url: "san-pham/{metatitle}-{productId}",
                defaults: new { controller = "Products", action = "ProductDetail" }
            );
            //------Default
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
