using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrangwebnuocmamThanhKhoa.Areas.Admin.Data;
using TrangwebnuocmamThanhKhoa.Models;
using Unidecode.NET;

namespace TrangwebnuocmamThanhKhoa.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        // GET: Admin/Product
        public ActionResult ListCategory()
        {
            var listcategory = db.Categories.ToList();
            ViewBag.CountCategory = listcategory.Count();
            return View(listcategory);
        }


        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateCategory(FormCollection f)
        {
            try
            {
                Category ct = new Category();
                ct.Name = f["Name"];
                ct.MetaTitle = f["Name"].Unidecode().Replace(" ", "-") + "-" + DateTime.Now.ToString("MMddyyHHmm");
                ct.Status = true;
                //ct.Display = true;
                db.Categories.InsertOnSubmit(ct);
                db.SubmitChanges();
                return RedirectToAction("ListCategory");

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
          
        }
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var categoryToDelete = db.Categories.SingleOrDefault(n => n.Id == id);
                if (categoryToDelete == null)
                {
                    ModelState.AddModelError("", "Danh mục không tồn tại.");
                    return RedirectToAction("ListCategory");
                }

                var productsToUpdate = db.Products.Where(p => p.CategoryId == id).ToList();
                foreach (var product in productsToUpdate)
                {
                    product.CategoryId = 0;
                }
                db.SubmitChanges();
                db.Categories.DeleteOnSubmit(categoryToDelete);
                db.SubmitChanges();

                return RedirectToAction("ListCategory");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi xử lý yêu cầu của bạn: " + ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult EditCategory (int id  )
        {
            var category = db.Categories.SingleOrDefault(n => n.Id == id);
            var product = db.Products.Where(n => n.CategoryId == id).ToList();
            ViewBag.listproduct = product;
            ViewBag.idcategory = id;
            return View(category);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditCategory (int id , FormCollection f)
        {
            try
            {
                var category = db.Categories.SingleOrDefault(n => n.Id == id);
                category.Name = f["Name"];
                category.MetaTitle = f["Name"].Unidecode().Replace(" ", "-") + "-" + DateTime.Now.ToString("MMddyyHHmm");
                db.SubmitChanges();
                return RedirectToAction("EditCategory", new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }

        }

        [HttpGet]
        public ActionResult ProductsWithoutCategory ()
        {

            var productsWithoutCategory = db.Products.Where(p => p.CategoryId == 0).ToList();
            var categories = db.Categories.ToList();

            ViewBag.Categories = categories;

            return View(productsWithoutCategory);
        }

        [HttpPost]
        public ActionResult RestoreProductCategory(int productId, int categoryId)
        {
            try
            {
                var product = db.Products.SingleOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    product.CategoryId = categoryId;
                    db.SubmitChanges();
                }

                TempData["RestoreSuccess"] = "Khôi phục danh mục thành công!";
                return RedirectToAction("ProductsWithoutCategory");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View("ProductsWithoutCategory", db.Products.Where(p => p.CategoryId == 0).ToList());
            }
        }

        public ActionResult CreateProduct(int idl)
        {
            ViewBag.idloai = idl;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateProduct(int idl, FormCollection f)
        {
            try
            {
                Product pd = new Product();
                pd.CategoryId = idl;
                pd.Name = f["Name"];
                pd.ProductList = "[]";
                pd.Description = f["NoiDung"];
                pd.Status = true;
                pd.CreateAt = DateTime.Now;
                pd.MetaTitle = f["Name"].Unidecode().Replace(" ", "-") + "-" + DateTime.Now.ToString("MMddyyHHmm");
                pd.SaleOff = int.Parse(f["SaleOff"]);
                pd.Images = f["HinhAnhThumbNail"];
                db.Products.InsertOnSubmit(pd);
                db.SubmitChanges();
                return RedirectToAction("EditCategory", new { id = idl });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }


        public ActionResult DeleteProduct(int id, int idl)
        {
            try
            {
                var delete = db.Products.SingleOrDefault(n => n.Id == id);
                db.Products.DeleteOnSubmit(delete);
                db.SubmitChanges();
                return RedirectToAction("EditCategory", new { id = idl });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }


        [HttpGet]
        public ActionResult EditProduct(int id, int idl)
        {
            var edit = db.Products.SingleOrDefault(n => n.Id == id);
            return View(edit);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditProduct(int id, int idl, FormCollection f)
        {
            try
            {
                var edit = db.Products.SingleOrDefault(n => n.Id == id);
                edit.Name = f["Name"];
                edit.Description = f["NoiDung"];
                edit.MetaTitle = f["Name"].Unidecode().Replace(" ", "-") + "-" + DateTime.Now.ToString("MMddyyHHmm");
                edit.SaleOff = int.Parse(f["SaleOff"]);
                edit.Images = f["HinhAnhThumbNail"];
                db.SubmitChanges();
                return RedirectToAction("EditCategory", new { id = idl });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while processing your request: " + ex.Message);
                return View();
            }
        }



        [HttpGet]
        public JsonResult GetProductList(int productId)
        {
            var product = db.Products.SingleOrDefault(n => n.Id == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" }, JsonRequestBehavior.AllowGet);
            }

            var productList = JsonConvert.DeserializeObject<List<ProductDetail>>(product.ProductList);

            return Json(new { success = true, data = productList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddProductToList(int productId, string Name, string Thetich, string Gia)
        {
            try
            {
                if ( string.IsNullOrEmpty(Thetich) || string.IsNullOrEmpty(Gia))
                {
                    return Json(new { success = false, message = "All fields are required." });
                }

                var product = db.Products.SingleOrDefault(n => n.Id == productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }

                // Deserialize existing product list
                var productList = JsonConvert.DeserializeObject<List<ProductDetail>>(product.ProductList) ?? new List<ProductDetail>();

                // Add new product to the list
                productList.Add(new ProductDetail
                {
                    Name = Name,
                    Thetich = int.Parse(Thetich),
                    Gia = int.Parse(Gia),
                    Status = true
                });

                // Serialize back to JSON and update the product
                product.ProductList = JsonConvert.SerializeObject(productList);
                db.SubmitChanges();

                return Json(new { success = true });
            }
            catch (FormatException ex)
            {
                return Json(new { success = false, message = "Invalid format: " + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult RemoveProductFromList(int productId, string Name)
        {
            try
            {
                var product = db.Products.SingleOrDefault(n => n.Id == productId);
                var productList = JsonConvert.DeserializeObject<List<ProductDetail>>(product.ProductList) ?? new List<ProductDetail>();
                // Find and remove the product from the list
                var productToRemove = productList.SingleOrDefault(p => p.Name == Name);
                productList.Remove(productToRemove);

                // Serialize back to JSON and update the product
                product.ProductList = JsonConvert.SerializeObject(productList);
                db.SubmitChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateStatusProductChild(int id, string Name, bool isChecked)
        {
            try
            {
                var prd = db.Products.SingleOrDefault(m => m.Id == id);
                if (prd != null)
                {
                    var prdlist = JsonConvert.DeserializeObject<List<ProductDetail>>(prd.ProductList) ?? new List<ProductDetail>();
                    var prdName = prdlist.SingleOrDefault(n => n.Name == Name);
                    if (prdName != null)
                    {
                        prdName.Status = isChecked;
                        prd.ProductList = JsonConvert.SerializeObject(prdlist);
                        db.SubmitChanges();
                        return Json(new { success = true, message = "Chuyển trạng thái thành công" });
                    }
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong danh sách" });
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái của sản phẩm." });
            }
        }


        [HttpPost]
        public ActionResult UpdateStatusCategory(int id, bool isChecked)
        {
            try
            {
                var check = db.Categories.SingleOrDefault(m => m.Id == id);
                if (isChecked == true)
                {
                    check.Status = true;
                }
                else
                {
                    check.Status = false;
                }
                db.SubmitChanges();
                return Json(new { success = true, message = "Chuyển trạng thái thành công" });
            }
            catch (Exception)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái của tin tức." });
            }
        }
        [HttpPost]
        public ActionResult UpdateStatusProduct(int id, bool isChecked)
        {
            try
            {
                var check = db.Products.SingleOrDefault(m => m.Id == id);
                if (isChecked == true)
                {
                    check.Status = true;
                }
                else
                {
                    check.Status = false;
                }
                db.SubmitChanges();
                return Json(new { success = true, message = "Chuyển trạng thái thành công" });
            }
            catch (Exception)
            {
                // Xử lý lỗi nếu có
                return Json(new { success = false, message = "Đã xảy ra lỗi khi cập nhật trạng thái của tin tức." });
            }
        }
    }
}