using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Newtonsoft.Json;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Model;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class ProductsController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        [Route("san-pham/main-page")]
        public ActionResult MainPage()
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.User = nd_dv;
            var category = _db.Categorys
                //.Where(x => x.StatusID == EnumStatus.ACTIVE)
                .ToList();
            string cbxCategory = "<option value=\"\">Danh mục sản phẩm</option>";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\">{1}</option>", item.CategoryId, item.Name);
            }
            ViewBag.cbxCategory = cbxCategory;

            return View();
        }

        [Route("san-pham/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, Guid? category = null, int sotrang = 1, int tongsodong = 5)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Products.ToList()
                        join b in _db.Categorys.ToList() on a.CategoryId equals b.CategoryId
                        where status == null ? true : a.StatusID == status
                        select new ProductInfo
                        {
                            Product = a,
                            Category = b

                        })
                        .OrderByDescending(x => x.Product.CreateDate)
                        .ToList();

            int tongso = list.Count();
            sotrang = sotrang <= 0 ? 1 : sotrang;
            tongsodong = tongsodong <= 0 ? 10 : tongsodong;
            int tongsotrang = tongso % tongsodong > 0 ? tongso / tongsodong + 1 : tongso / tongsodong;
            tongsotrang = tongsotrang <= 0 ? 1 : tongsotrang;
            sotrang = sotrang > tongsotrang ? tongsotrang - 1 : sotrang - 1;
            ViewBag.sotrang = sotrang + 1;
            ViewBag.tongsotrang = tongsotrang;
            ViewBag.tongso = tongso;
            return PartialView(list == null ? list : list.Skip(sotrang * tongsodong).Take(tongsodong));
        }

        [Route("san-pham/update")]
        public ActionResult Update(Guid? id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            var category = _db.Categorys
                .Where(x => x.StatusID == EnumStatus.ACTIVE)
                .ToList();
            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);

            string cbxCategory = "<option value=\"\">Danh mục sản phẩm</option>";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.CategoryId, item.Name, products != null && products.CategoryId == item.CategoryId ? "selected" : "");
            }
            ViewBag.cbxCategory = cbxCategory;

            var sizes = _db.Sizes.Where(x => x.StatusID == EnumStatus.ACTIVE).ToList();
            var listSize = new List<int?>();
            var listProductSize = new List<ProductSize>();
            if (products != null)
            {
                listProductSize = _db.ProductSizes.Where(x => x.ProductId == products.ProductId).ToList();
                listSize = listProductSize.Select(x => x.SizeId).Where(x => x != null).ToList();
            }
            string cbxSize = "<option value=\"\">Danh mục kích thước</option>";
            foreach (var item in sizes)
            {
                cbxSize += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.SizeId, item.Name, products != null && listSize.Contains(item.SizeId) ? "selected" : "");
            }
            ViewBag.cbxSize = cbxSize;

            var ingredients = _db.Ingredients.Where(x => x.StatusID == EnumStatus.ACTIVE).ToList();
            var listProductIngredient = new List<ProductIngredient>();
            var listIngredient = new List<int?>();
            if (products != null)
            {
                listProductIngredient = _db.ProductIngredients.Where(x => x.ProductId == products.ProductId).ToList();
                listIngredient = listProductIngredient.Select(x => x.IngredientId).Where(x => x != null).ToList();
            }
            string cbxIngredient = "<option value=\"\">Danh mục nguyên liệu</option>";
            foreach (var item in ingredients)
            {
                cbxIngredient += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.IngredientId, item.Name, products != null && listSize.Contains(item.IngredientId) ? "selected" : "");
            }
            ViewBag.cbxIngredient = cbxIngredient;

            ViewBag.ProductIngredien = listProductIngredient;
            ViewBag.ProductSize = listProductSize;
            var listDirection = new List<ProductDirection>();
            if (products != null)
            {
                listDirection = _db.ProductDirections.Where(x => x.ProductId == products.ProductId).ToList();
            }
            ViewBag.Direction = listDirection;
            return PartialView(products);
        }

        [Route("san-pham/update")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Product products, string Price = "", HttpPostedFileBase _Logo = null, HttpPostedFileBase _BackGround = null, HttpPostedFileBase _videoUrl = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == products.CategoryId);
            if (category == null)
            {
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));
            }
            if (products.ProductId == Guid.Empty)
            {
                //var checkTontai = _db.Sliders.FirstOrDefault(x => x.StatusID == EnumStatus.ACTIVE && x.STT == slider.STT);
                //if (checkTontai != null)
                //    return Json(new { kq = "err", msg = Translate.SLIDER_STT_IS_USED }, JsonRequestBehavior.AllowGet);

                // tao moi
                products.ProductId = Guid.NewGuid();
                products.Code = Helpers.CreateCode6Char();
                products.StatusID = EnumStatus.ACTIVE;
                products.CreateDate = DateTime.Now;
                products.IsNew = EnumStatus.ACTIVE;
                if (_Logo != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    products.Image = fileImage[1];
                }
                if (_BackGround != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _BackGround.uploadFile(rootPathImage, filePathImage);
                    products.Background = fileImage[1];
                }
                //if (_videoUrl != null)
                //{
                //    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                //    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                //    string[] fileImage = _videoUrl.uploadFile(rootPathImage, filePathImage);
                //    products.Image = fileImage[1];
                //}
                _db.Products.Add(products);
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                // up date
                var old = _db.Products.FirstOrDefault(x => x.ProductId == products.ProductId);
                if (old == null)
                {
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
                }
                if (_Logo != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    old.Image = fileImage[1];
                }
                if (_BackGround != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _BackGround.uploadFile(rootPathImage, filePathImage);
                    old.Background = fileImage[1];
                }
                old.CategoryId = products.CategoryId;
                old.Name = products.Name;
                old.IsNew = products.IsNew;
                old.VideoUrl = products.VideoUrl;
                old.VideoTitle = products.VideoTitle;
                old.VideoDescription = products.VideoDescription;
                old.CategoryId = products.CategoryId;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("san-pham/delete")]
        public ActionResult Delete(Guid id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
            products.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)));
        }

        [Route("san-pham/change-status")]
        public ActionResult Change_Status(Guid id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));

            if (products.StatusID == EnumStatus.ACTIVE)
                products.StatusID = EnumStatus.INACTIVE;
            else
                products.StatusID = EnumStatus.ACTIVE;
            _db.SaveChanges();
            return Json(new CxResponse<object>(products.StatusID, Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
        }

        [Route("san-pham/get-by-categories")]
        public ActionResult GetProductByCategory(string id, string product, string selectId = "", string isMulti = "")
        {
            var products = _db.Products.Where(x => id.Contains(x.CategoryId.ToString()) && x.StatusID == EnumStatus.ACTIVE).ToList();
            string select = string.Format("<select class='ctr-select' data-live-search='true' name=\"{0}\" {1} >", selectId, string.IsNullOrEmpty(isMulti) ? "" : "multiple");
            string cbxProduct = "<option value=\"\">Danh mục sản phẩm</option>";
            foreach (var item in products)
            {
                cbxProduct += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.ProductId, item.Name, !string.IsNullOrEmpty(product) && product.Contains(item.ProductId.ToString()) ? "selected" : "");
            }
            var obj = new
            {
                data = cbxProduct
            };
            ViewBag.Category = select + cbxProduct + "<select>";
            return PartialView();
            //return Json(new CxResponse<object>(obj));
        }




        [Route("san-pham/update-step")]
        public ActionResult UpdateStep(Guid? productId, Guid? directionId)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            var productDirection = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == directionId);
            return PartialView(productDirection);
        }
        [Route("san-pham/update-step")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateStep(ProductDirection direction, HttpPostedFileBase _LogoStep = null, Guid? ProductTempId = null)
        {
            if (direction.ProductDirectionId == Guid.Empty)
            {
                direction.ProductDirectionId = Guid.NewGuid();
                direction.CreateDate = DateTime.Now;
                direction.StatusID = EnumStatus.ACTIVE;
                direction.ProductId = ProductTempId;
                direction.Code = Helpers.CreateCode6Char();
                if (_LogoStep != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _LogoStep.uploadFile(rootPathImage, filePathImage);
                    direction.Image = fileImage[1];
                }

                _db.ProductDirections.Add(direction);
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                // up date
                var old = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == direction.ProductDirectionId);
                if (old == null)
                {
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
                }
                if (_LogoStep != null)
                {
                    string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _LogoStep.uploadFile(rootPathImage, filePathImage);
                    old.Image = fileImage[1];
                }
                old.Name = direction.Name;
                old.SortOrder = direction.SortOrder;
                old.Description = direction.Description;
                //old.ProductId = direction.ProductId;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }


        [Route("san-pham/list-step")]
        public ActionResult ListStep(Guid? productId)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            var productDirection = _db.ProductDirections.Where(x => x.ProductId == productId);
            return PartialView(productDirection);
        }
    }
}