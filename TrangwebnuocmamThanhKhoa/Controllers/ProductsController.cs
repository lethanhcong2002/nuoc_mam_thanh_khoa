using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Models;
using Newtonsoft.Json;

namespace TrangwebnuocmamThanhKhoa.Controllers
{
    public class ProductsController : Controller
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        // GET: Products
        public ActionResult ProductList(string query)
        {
            ViewBag.SearchQuery = query;
            var products = GetAllProducts();
            return View(products);
        }

        private List<PDDetail> GetAllProducts()
        {
            var products = db.Products
                     .Where(p => p.Status == true)
                     .OrderByDescending(p => p.CreateAt)
                     .ToList();

            var productDetail = new List<PDDetail>();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle,
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }
            return productDetail;
        }


        public ActionResult NewProducts()
        {
            var newProducts = GetNewProducts();
            return PartialView("NewProductPartial", newProducts);
        }

        private List<PDDetail> GetNewProducts()
        {
            // Lấy tất cả sản phẩm với trạng thái là true và sắp xếp theo ngày tạo giảm dần
            var products = db.Products
                     .Where(p => p.Status == true)
                     .OrderByDescending(p => p.CreateAt)
                     .ToList(); // Lấy tất cả sản phẩm trước

            // Danh sách sản phẩm chi tiết sau khi xử lý
            var productDetail = new List<PDDetail>();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    // Kiểm tra nếu danh sách sản phẩm không rỗng
                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle,
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }

            // Lấy 5 sản phẩm đầu tiên sau khi đã xử lý
            return productDetail.Take(5).ToList();
        }

        public ActionResult Categories()
        {
            var categories = GetFishSauceCategories();
            return PartialView("CategoryProductPartial", categories);
        }

        private List<Category> GetFishSauceCategories()
        {
            var categories = db.Categories.Where(p => p.Status == true).ToList();
            return categories;
        }

        [HttpGet]
        public ActionResult FilterAndSortProducts(string sortBy, string priceOrder, string keyword, int? price1, int? price2, int page = 1, int limit = 10)
        {
            var products = GetFilteredAndSortedProducts(sortBy, priceOrder, keyword, price1, price2);

            var pagedProducts = products.Skip((page - 1) * limit).Take(limit).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count / (double)limit);
            ViewBag.CurrentPage = page;
            return PartialView("ProductListPartial", pagedProducts);
        }

        private List<PDDetail> GetFilteredAndSortedProducts(string sortBy, string priceOrder, string keyword, int? price1, int? price2)
        {
            var query = db.Products.AsQueryable();

            query = query.Where(p => p.Status == true);

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }

            // Sắp xếp theo tiêu chí đầu tiên
            if (sortBy == "newest")
            {
                query = query.OrderByDescending(p => p.CreateAt);
            }
            else if (sortBy == "bestselling")
            {
                // Add logic to order by best selling if applicable
            }

            var products = query.ToList();
            var productDetail = new List<PDDetail>();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }

            // Lọc theo giá
            if (price1.HasValue && price2.HasValue && price1.Value != 0 && price2.Value != 0)
            {
                productDetail = productDetail.Where(p => p.Price >= price1.Value && p.Price <= price2.Value).ToList();
            }

            // Áp dụng sắp xếp theo giá 
            if (priceOrder == "asc1")
            {
                productDetail = productDetail.OrderBy(p => p.Price).ToList();
            }
            else if (priceOrder == "desc1")
            {
                productDetail = productDetail.OrderByDescending(p => p.Price).ToList();
            }
            return productDetail;
        }


        public ActionResult FilterByCategory(int categoryId, int page = 1, int limit = 10)
        {
            var products = GetProductsByCategory(categoryId);
            var pagedProducts = products.Skip((page - 1) * limit).Take(limit).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count / (double)limit);
            ViewBag.CurrentPage = page;
            return PartialView("ProductListPartial", pagedProducts);
        }

        private List<PDDetail> GetProductsByCategory(int categoryId)
        {
            var products = db.Products
                              .Where(p => p.CategoryId == categoryId && p.Status == true)
                              .ToList();
            var productDetail = new List<PDDetail>();

            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle,
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }

            return productDetail;
        }


        public ActionResult ProductDetail(string metatitle, int productId)
        {
            var products = db.Products.Where(id => id.Id == productId && id.Status == true).FirstOrDefault();  // Lọc sản phẩm theo trạng thái

            if (products == null)
            {
                return HttpNotFound();
            }

            var images = !string.IsNullOrEmpty(products.Images) ? products.Images.Split(';').ToList() : new List<string>();

            var productdetail = new PdDetail1
            {
                Id = productId,
                Name = products.Name,
                Images = images,
                Price = 0,
                MetaTitle = metatitle,
                ProductListDetails = new List<ProductListDetail>()
            };

            if (!string.IsNullOrEmpty(products.ProductList))
            {
                var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(products.ProductList);
                if (productItems != null && productItems.Any())
                {
                    productdetail.Price = (int)productItems.First().Gia;
                    productdetail.ProductListDetails = productItems;
                }
            }
            return View(productdetail);
        }

        public ActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("ProductList");
            }

            ViewBag.SearchQuery = query;

            return RedirectToAction("ProductList", new { query = query });
        }

        public ActionResult OtherProduct()
        {
            var orderProducts = GetOtherProducts();
            return PartialView("OtherProductPartial", orderProducts);
        }

        private List<PDDetail> GetOtherProducts()
        {
            var products = db.Products
                     .Where(p => p.Status == true)
                     .OrderByDescending(p => p.CreateAt)
                     .ToList();

            var productDetail = new List<PDDetail>();
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.ProductList))
                {
                    var productItems = JsonConvert.DeserializeObject<List<ProductListDetail>>(product.ProductList);

                    if (productItems != null && productItems.Any())
                    {
                        var productdetail = new PDDetail
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Images = !string.IsNullOrEmpty(product.Images) ? product.Images.Split(';').First() : null,
                            Price = (int)productItems.First().Gia,
                            MetaTitle = product.MetaTitle,
                        };
                        productDetail.Add(productdetail);
                    }
                }
            }
            return productDetail.Take(8).ToList();
        }
    }
}
