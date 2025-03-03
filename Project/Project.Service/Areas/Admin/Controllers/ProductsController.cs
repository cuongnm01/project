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
using System.Web.UI;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class ProductsController : BaseController
    {
        AppDbContext _db = new AppDbContext();

        #region Recipe
        [Route("recipe/main-page")]
        public ActionResult MainPage(int? categoryId = null)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            ViewBag.categoryId = categoryId ?? 0;
            return View();
        }

        [Route("recipe/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int? category = null, int sotrang = 1, int tongsodong = 5)
        {

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Products.ToList()
                        join b in _db.Categorys.ToList() on a.CategoryId equals b.CategoryId
                        where (status == null ? true : a.StatusID == status) &&
                                (category == null || category == 0 ? true : a.CategoryId == category) &&
                                (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new ProductInfo
                        {
                            Product = a,
                            Category = b

                        })
                        .OrderBy(x => x.Category.Name).ThenBy(x => x.Product.Name)
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

        [Route("recipe/insert")]
        public ActionResult Insert(Guid? id)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var product = new Product();
            var category = _db.Categorys
                .Where(x => x.StatusID == EnumStatus.ACTIVE)
                .ToList();
            string cbxCategory = "";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\">{1}</option>", item.CategoryId, item.Name);
            }
            ViewBag.cbxCategory = cbxCategory;

            return PartialView(product);
        }

        [Route("recipe/insert")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(Product products, bool isConfirm = false)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var exist = _db.Products.Count(x => x.Name.ToLower() == products.Name.ToLower().Trim() && x.StatusID == EnumStatus.ACTIVE);
            if (exist > 0 && isConfirm == false)
            {
                return Json(new CxResponse("warning", string.Format("Recipe {0} is dupplicated", products.Name)));
            }

            if (products.ProductId == Guid.Empty)
            {
                // tao moi
                products.Code = Helpers.CreateCode6Char();
                products.StatusID = EnumStatus.ACTIVE;
                products.CreateDate = DateTime.Now;
                products.IsNew = EnumStatus.ACTIVE;
                products.ProductId = Guid.NewGuid();
                _db.Products.Add(products);
                _db.SaveChanges();

                return Json(new CxResponse<object>(products.ProductId, Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                // up date
                var old = _db.Products.FirstOrDefault(x => x.ProductId == products.ProductId);
                if (old == null)
                {
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
                }

                old.CategoryId = products.CategoryId;
                old.Name = products.Name;
                old.IsNew = products.IsNew;
                old.VideoUrl = products.VideoUrl;
                old.VideoTitle = products.VideoTitle;
                old.VideoDescription = products.VideoDescription;
                old.CategoryId = products.CategoryId;
                old.CreateDate = DateTime.Now;
                _db.SaveChanges();
                return Json(new CxResponse<object>(old.ProductId, Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe/update")]
        public ActionResult Update(Guid? id)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var category = _db.Categorys
                .Where(x => x.StatusID == EnumStatus.ACTIVE)
                .ToList();
            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);

            string cbxCategory = "<option value=\"\">Recipe Books</option>";
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
            string cbxSize = "";
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
            string cbxIngredient = "";
            foreach (var item in ingredients)
            {
                cbxIngredient += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.IngredientId, item.Name, products != null && listIngredient.Contains(item.IngredientId) ? "selected" : "");
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

        [Route("recipe/update")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(Product products, HttpPostedFileBase _Logo = null, HttpPostedFileBase _BackGround = null, HttpPostedFileBase _videoUrl = null)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == products.CategoryId);
            if (category == null)
            {
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));
            }
            // up date
            var old = _db.Products.FirstOrDefault(x => x.ProductId == products.ProductId);
            if (old == null)
            {
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
            }
            if (_Logo != null)
            {
                string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("yyyy/MM/dd"));
                string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                old.Image = fileImage[1];
            }
            if (_BackGround != null)
            {
                string rootPathImage = string.Format("~/Files/products/{0}", DateTime.Now.ToString("yyyy/MM/dd"));
                string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                string[] fileImage = _BackGround.uploadFile(rootPathImage, filePathImage);
                old.Background = fileImage[1];
            }

            old.CategoryId = products.CategoryId;
            old.Name = products.Name;
            old.VideoUrl = products.VideoUrl;
            old.VideoTitle = products.VideoTitle;
            old.VideoDescription = products.VideoDescription;
            old.CategoryId = products.CategoryId;
            old.CreateDate = DateTime.Now;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
        }

        [Route("recipe/delete")]
        public ActionResult Delete(Guid id)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);
            products.StatusID = EnumStatus.DELETE;
            //Remove all
            _db.Products.Remove(products);

            var stepGroups = _db.ProductDirectionGroup.Where(x => x.ProductId == id);
            if (stepGroups != null && stepGroups.Count() > 0)
                _db.ProductDirectionGroup.RemoveRange(stepGroups);

            var steps = _db.ProductDirections.Where(x => x.ProductId == id);
            if (steps != null && steps.Count() > 0)
                _db.ProductDirections.RemoveRange(steps);

            var ingredientGroups = _db.ProductIngredientGroup.Where(x => x.ProductId == id);
            if (ingredientGroups != null && ingredientGroups.Count() > 0)
                _db.ProductIngredientGroup.RemoveRange(ingredientGroups);


            var ingredients = _db.ProductIngredients.Where(x => x.ProductId == id);
            if (ingredients != null && ingredients.Count() > 0)
                _db.ProductIngredients.RemoveRange(ingredients);

            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("recipe/delete-image")]
        public ActionResult DeleteImage(Guid id, int? type = null)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            if (type == EnumProductImageType.IMAGE)
            {
                products.Image = null;
            }
            else
            {
                products.Background = null;
            }
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }


        [Route("recipe/delete-all")]
        public ActionResult DeleteAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var productsIds = ids.Split(',').ToList();

            var products = _db.Products.Where(x => productsIds.Contains(x.ProductId.ToString()));
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            _db.Products.RemoveRange(products);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion



        #region Add Step to Recipe
        [Route("recipe/list-step")]
        public ActionResult ListStep(Guid? productId)
        {
            var productDirection = (from a in _db.ProductDirections
                                    join c in _db.ProductDirectionGroup on a.ProductDirectionGroupId equals c.ProductDirectionGroupId into c1
                                    from c in c1.DefaultIfEmpty()
                                    where a.ProductId == productId
                                    select new ProductDirectionInfo()
                                    {
                                        Header = c != null ? c.Name : "",
                                        ProductDirection = a,
                                        ProductDirectionGroup = c,
                                    }).OrderBy(x => x.ProductDirectionGroup != null ? x.ProductDirectionGroup.SortOrder : 0).ThenBy(x => x.ProductDirection.SortOrder);

            return PartialView(productDirection);
        }

        [Route("recipe/update-step")]
        public ActionResult UpdateStep(Guid? productId, Guid? directionId)
        {
            var new_record = new ProductDirection();
            var sortOrder = _db.ProductDirections.Where(x => x.ProductId == productId).OrderByDescending(x => x.SortOrder).Select(x => x.SortOrder).FirstOrDefault();
            new_record.SortOrder = sortOrder != null ? (sortOrder + 1) : 1;

            var obj = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == directionId);
            obj = obj == null ? new_record : obj;

            var headers = _db.ProductDirectionGroup.Where(x => x.ProductId == obj.ProductId && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.Name).ToList();
            string cbxHeaders = "";
            foreach (var item in headers)
            {
                cbxHeaders += string.Format("<option value=\"{0}\">{1}</option>", item.ProductDirectionGroupId, item.Name);
            }
            ViewBag.cbxHeaders = cbxHeaders;

            return PartialView(obj);

        }

        [Route("recipe/update-step")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateStep(ProductDirection direction, HttpPostedFileBase _LogoStep = null, Guid? ProductTempId = null)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == ProductTempId);

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

                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }

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
                old.ProductDirectionGroupId = direction.ProductDirectionGroupId;
                old.Name = direction.Name;
                old.Description = direction.Description;

                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe/change-data-step")]
        public ActionResult ChangeDataStep(Guid? id = null, dynamic value = null, int? type = null)
        {

            var obj = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == id);
            if (obj != null)
            {

                if (type == EnumStepData.SORT_ORDER)
                {
                    obj.SortOrder = Convert.ToInt32(value[0] as string);
                }
                else if (type == EnumStepData.DESCRIPTION)
                {
                    obj.Description = value[0] as string;
                }
                var product = _db.Products.FirstOrDefault(x => x.ProductId == obj.ProductId);
                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }
        }

        [Route("recipe/delete-step")]
        public ActionResult DeleteStep(Guid id)
        {
            var productsDirection = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == id);
            if (productsDirection == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
            _db.ProductDirections.Remove(productsDirection);
            var product = _db.Products.FirstOrDefault(x => x.ProductId == productsDirection.ProductId);
            if (product != null)
            {
                product.CreateDate = DateTime.Now;
            }
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("recipe/delete-step-image")]
        public ActionResult DeleteStepImage(Guid id)
        {

            var productsDirection = _db.ProductDirections.FirstOrDefault(x => x.ProductDirectionId == id);
            if (productsDirection == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);
            productsDirection.Image = null;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Add Step group to Recipe
        [Route("recipe/update-step-group")]
        public ActionResult UpdateStepGroup(Guid? id = null, Guid? productId = null)
        {
            var new_record = new ProductDirectionGroup();
            new_record.ProductId = productId;

            var obj = _db.ProductDirectionGroup.FirstOrDefault(x => x.ProductDirectionGroupId == id);
            obj = obj == null ? new_record : obj;

            return PartialView(obj);
        }

        [Route("recipe/update-step-group")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateStepGroup(ProductDirectionGroup obj)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == obj.ProductId);

            var productDirectionGroup = _db.ProductDirectionGroup.FirstOrDefault(x => x.ProductId == obj.ProductId && x.Name.ToLower() == obj.Name.ToLower().Trim());
            if (productDirectionGroup != null)
            {
                return Json(new CxResponse("err", Message.MSG_USED.Params(productDirectionGroup.Name)));
            }

            if (obj.ProductDirectionGroupId == Guid.Empty)
            {
                obj.ProductDirectionGroupId = Guid.NewGuid();
                obj.CreateDate = DateTime.Now;
                obj.StatusID = EnumStatus.ACTIVE;
                obj.SortOrder = _db.ProductDirectionGroup.Count(x => x.ProductId == obj.ProductId) + 1;
                _db.ProductDirectionGroup.Add(obj);
                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                var old = _db.ProductDirectionGroup.FirstOrDefault(x => x.ProductDirectionGroupId == obj.ProductDirectionGroupId);
                if (old == null)
                    return Json(new CxResponse("err", Message.MSG_EMPTY));

                old.Name = obj.Name;
                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe/change-data-step-group")]
        public ActionResult ChangeDataStepGroup(Guid? id = null, int? value = null)
        {
            var obj = _db.ProductDirectionGroup.FirstOrDefault(x => x.ProductDirectionGroupId == id);
            if (obj != null)
            {
                obj.SortOrder = value;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }
        }

        [Route("recipe/delete-step-group")]
        public ActionResult DeleteStepGroup(Guid id)
        {
            var productDirectionGroup = _db.ProductDirectionGroup.FirstOrDefault(x => x.ProductDirectionGroupId == id);
            if (productDirectionGroup == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("Header")));
            _db.ProductDirectionGroup.Remove(productDirectionGroup);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add Step to Recipe by text area
        [Route("recipe/update-step-text")]
        public ActionResult UpdateStepByText(Guid? productId = null, int? sizeId = null)
        {
            ProductDirection productDirection = new ProductDirection();
            productDirection.ProductId = productId;

            return PartialView(productDirection);
        }

        [Route("recipe/update-step-text")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateStepByText(ProductDirection obj, Guid? ProductTempId = null, string stepItems = "")
        {
            string[] lines = stepItems.Trim().Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            var sortOrder = _db.ProductDirections.Where(x => x.ProductId == obj.ProductId).OrderByDescending(x => x.SortOrder).Select(x => x.SortOrder).FirstOrDefault() ?? 0;
            var productDirectionGroups = _db.ProductDirectionGroup.Where(x => x.StatusID != EnumStatus.DELETE && x.ProductId == ProductTempId);

            List<ProductDirection> lstProductDirections = new List<ProductDirection>();
            List<ProductDirectionGroup> lstProductDirectionGroups = new List<ProductDirectionGroup>();

            Guid? currentHeader = null;

            foreach (var item in lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (item.Contains(":"))
                    {
                        var header = item.Replace(":", "").Trim();

                        var oldHeader = productDirectionGroups.FirstOrDefault(x => x.ProductId == obj.ProductId && x.Name.ToLower() == header.ToLower());
                        if (oldHeader == null)
                        {
                            ProductDirectionGroup productDirectionGroup = new ProductDirectionGroup();
                            productDirectionGroup.ProductDirectionGroupId = Guid.NewGuid();
                            productDirectionGroup.ProductId = ProductTempId;
                            productDirectionGroup.Name = header;
                            productDirectionGroup.StatusID = EnumStatus.ACTIVE;
                            productDirectionGroup.SortOrder = productDirectionGroups.Count() + lstProductDirectionGroups.Count() + 1;
                            productDirectionGroup.CreateDate = DateTime.Now;
                            lstProductDirectionGroups.Add(productDirectionGroup);
                            currentHeader = productDirectionGroup.ProductDirectionGroupId;
                        }
                        else
                        {
                            currentHeader = oldHeader.ProductDirectionGroupId;
                        }
                    }
                    else
                    {
                        sortOrder++;

                        ProductDirection direction = new ProductDirection();
                        direction.ProductDirectionId = Guid.NewGuid();
                        direction.CreateDate = DateTime.Now;
                        direction.StatusID = EnumStatus.ACTIVE;
                        direction.ProductId = ProductTempId;
                        direction.ProductDirectionGroupId = currentHeader;
                        direction.Code = Helpers.CreateCode6Char();
                        direction.SortOrder = sortOrder;
                        direction.Description = item;
                        lstProductDirections.Add(direction);
                    }
                }
            }
            if (lstProductDirections != null && lstProductDirections.Count() > 0)
            {
                _db.ProductDirections.AddRange(lstProductDirections);
                _db.ProductDirectionGroup.AddRange(lstProductDirectionGroups);

                var product = _db.Products.FirstOrDefault(x => x.ProductId == ProductTempId);
                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();
            }
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
        }
        #endregion

        #region Add Ingredient to Recipe
        [Route("recipe/ingredient")]
        public ActionResult RecipeIngredient(Guid? productId = null, int? sizeId = null)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == productId);

            var productIngredients = (from a in _db.ProductIngredients
                                      join b in _db.Ingredients on a.IngredientId equals b.IngredientId
                                      join c in _db.ProductIngredientGroup on a.ProductIngredientGroupId equals c.ProductIngredientGroupId into c1
                                      from c in c1.DefaultIfEmpty()
                                      where a.ProductId == productId && a.SizeId == sizeId
                                      select new ProductIngredientInfo()
                                      {
                                          Header = c != null ? c.Name : "",
                                          ProductIngredient = a,
                                          Ingredient = b,
                                          ProductIngredientGroup = c,
                                      }).OrderBy(x => x.ProductIngredientGroup != null ? x.ProductIngredientGroup.SortOrder : 0).ThenBy(x => x.ProductIngredient.SortOrder);

            var listUnit = _db.Units.Where(x => x.StatusID == EnumStatus.ACTIVE).ToList();
            ViewBag.Units = listUnit;

            return PartialView(productIngredients);
        }

        [Route("recipe/update-ingredient")]
        public ActionResult UpdateIngredient(Guid? id = null, Guid? productId = null, int? sizeId = null)
        {

            var new_record = new ProductIngredient();
            new_record.ProductId = productId;
            new_record.SizeId = sizeId;

            var obj = _db.ProductIngredients.FirstOrDefault(x => x.ProductIngredientId == id);
            obj = obj == null ? new_record : obj;

            var lstIngredient = _db.ProductIngredients.Where(x => x.ProductId == obj.ProductId && x.SizeId == sizeId).Select(x => x.IngredientId).ToList();

            var ingredients = _db.Ingredients.Where(x => id != null ? true : !lstIngredient.Contains(x.IngredientId) && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.Name).ToList();
            string cbxIngredient = "";
            foreach (var item in ingredients)
            {
                cbxIngredient += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.IngredientId, item.Name, obj.IngredientId == item.IngredientId ? "selected" : "");
            }
            ViewBag.cbxIngredient = cbxIngredient;

            var headers = _db.ProductIngredientGroup.Where(x => x.ProductId == obj.ProductId && x.StatusID == EnumStatus.ACTIVE).OrderBy(x => x.Name).ToList();
            string cbxHeaders = "";
            foreach (var item in headers)
            {
                cbxHeaders += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.ProductIngredientGroupId, item.Name, obj.ProductIngredientGroupId == item.ProductIngredientGroupId ? "selected" : "");
            }
            ViewBag.cbxHeaders = cbxHeaders;

            var ingredient = _db.Ingredients.FirstOrDefault(x => x.IngredientId == obj.IngredientId);
            if (ingredient != null)
            {
                var lstUnits = _db.Units.Where(x => x.UnitGroupId == ingredient.UnitGroupId).OrderBy(x => x.SortOrder).ToList();
                string cbxUnits = "";
                foreach (var item in lstUnits)
                {
                    cbxUnits += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.UnitId, item.Name, obj.UnitId == item.UnitId ? "selected" : "");
                }
                ViewBag.cbxUnits = cbxUnits;
            }

            return PartialView(obj);
        }

        [Route("recipe/update-ingredient")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateIngredient(ProductIngredient obj, Guid? ProductTempId = null)
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == ProductTempId);

            var ingredient = _db.Ingredients.FirstOrDefault(x => x.IngredientId == obj.IngredientId);
            if (ingredient == null)
            {
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("Ingredients")));
            }

            obj.Value = obj.Value ?? 0;

            if (obj.ProductIngredientId == Guid.Empty)
            {
                var checkExist = _db.ProductIngredients.Count(x => x.ProductId == ProductTempId && x.SizeId == obj.SizeId);
                if (checkExist > 9)
                {
                    return Json(new CxResponse("err", "Dupplicate Ingredient"));
                }

                var lastItem = _db.ProductIngredients.Where(x => x.ProductId == ProductTempId && x.SizeId == obj.SizeId && x.ProductIngredientGroupId == obj.ProductIngredientGroupId).OrderByDescending(x => x.SortOrder).FirstOrDefault();
                obj.ProductIngredientId = Guid.NewGuid();
                obj.CreateDate = DateTime.Now;
                obj.StatusID = EnumStatus.ACTIVE;
                obj.ProductId = ProductTempId;
                obj.SortOrder = (lastItem != null ? lastItem.SortOrder : 0) + 1;

                obj.Price = 0;
                var unit = _db.Units.FirstOrDefault(x => x.UnitId == obj.UnitId);
                if (unit != null)
                {
                    obj.Unit = unit.Name;

                    var unitBase = _db.Units.FirstOrDefault(x => x.UnitGroupId == unit.UnitGroupId && x.IsDefault == true);
                    var percent = (double)(unit.Rate / unitBase.Rate);
                    var price = obj.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);
                    obj.Price = price;
                }

                _db.ProductIngredients.Add(obj);

                if (obj.SizeId == EnumSize.REGULAR)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == ProductTempId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.LARGE);
                    if (checkSize == null)
                    {
                        var sizeLarge = new ProductIngredient();
                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                        sizeLarge.ProductId = obj.ProductId;
                        sizeLarge.SizeId = EnumSize.LARGE;
                        sizeLarge.IngredientId = obj.IngredientId;
                        sizeLarge.Value = obj.Value * 1.5;
                        sizeLarge.Price = obj.Price * 1.5;
                        sizeLarge.StatusID = obj.StatusID;
                        sizeLarge.CreateDate = obj.CreateDate;
                        sizeLarge.UnitId = obj.UnitId;
                        sizeLarge.Unit = obj.Unit;
                        sizeLarge.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeLarge.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeLarge);
                    }

                }
                else if (obj.SizeId == EnumSize.LARGE)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == ProductTempId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.REGULAR);
                    if (checkSize == null)
                    {
                        var sizeLarge = new ProductIngredient();
                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                        sizeLarge.ProductId = obj.ProductId;
                        sizeLarge.SizeId = EnumSize.REGULAR;
                        sizeLarge.IngredientId = obj.IngredientId;
                        sizeLarge.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        sizeLarge.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                        sizeLarge.StatusID = obj.StatusID;
                        sizeLarge.CreateDate = obj.CreateDate;
                        sizeLarge.UnitId = obj.UnitId;
                        sizeLarge.Unit = obj.Unit;
                        sizeLarge.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeLarge.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeLarge);
                    }
                }

                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                var old = _db.ProductIngredients.FirstOrDefault(x => x.ProductIngredientId == obj.ProductIngredientId);
                if (old == null)
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("Ingredients")));

                var checkExist = _db.ProductIngredients.Count(x => x.ProductId == ProductTempId && x.SizeId == obj.SizeId && x.ProductIngredientId != obj.ProductIngredientId);
                if (checkExist > 9)
                {
                    return Json(new CxResponse("err", "Dupplicate Ingredient"));
                }

                old.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                old.IngredientId = obj.IngredientId;
                old.Value = obj.Value;
                old.UnitId = obj.UnitId;
                old.Price = 0;
                var unit = _db.Units.FirstOrDefault(x => x.UnitId == obj.UnitId);
                if (unit != null)
                {
                    old.Unit = unit.Name;

                    var unitBase = _db.Units.FirstOrDefault(x => x.UnitGroupId == unit.UnitGroupId && x.IsDefault == true);
                    var percent = (double)(unit.Rate / unitBase.Rate);
                    var price = obj.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);
                    old.Price = price;
                }

                if (obj.SizeId == EnumSize.REGULAR)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == ProductTempId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.LARGE);
                    if (checkSize == null)
                    {
                        var sizeLarge = new ProductIngredient();
                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                        sizeLarge.ProductId = obj.ProductId;
                        sizeLarge.SizeId = EnumSize.REGULAR;
                        sizeLarge.IngredientId = obj.IngredientId;
                        sizeLarge.Value = obj.Value * 1.5;
                        sizeLarge.Price = obj.Price * 1.5;
                        sizeLarge.StatusID = obj.StatusID;
                        sizeLarge.CreateDate = obj.CreateDate;
                        sizeLarge.UnitId = obj.UnitId;
                        sizeLarge.Unit = obj.Unit;
                        sizeLarge.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeLarge.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeLarge);
                    }
                    else
                    {
                        checkSize.Value = obj.Value * 1.5;
                        checkSize.Price = obj.Price * 1.5;
                    }
                }
                else if (obj.SizeId == EnumSize.LARGE)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == ProductTempId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.REGULAR);
                    if (checkSize == null)
                    {
                        var sizeLarge = new ProductIngredient();
                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                        sizeLarge.ProductId = obj.ProductId;
                        sizeLarge.SizeId = EnumSize.REGULAR;
                        sizeLarge.IngredientId = obj.IngredientId;
                        sizeLarge.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        sizeLarge.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                        sizeLarge.StatusID = obj.StatusID;
                        sizeLarge.CreateDate = obj.CreateDate;
                        sizeLarge.UnitId = obj.UnitId;
                        sizeLarge.Unit = obj.Unit;
                        sizeLarge.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeLarge.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeLarge);
                    }
                    else
                    {
                        checkSize.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        checkSize.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                    }
                }

                if (product != null)
                {
                    product.CreateDate = DateTime.Now;
                }
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe/change-data-ingredient")]
        public ActionResult ChangeDataIngredient(Guid? id = null, double? value = null, int? type = null)
        {
            var obj = _db.ProductIngredients.FirstOrDefault(x => x.ProductIngredientId == id);
            if (obj != null)
            {
                var ingredient = _db.Ingredients.FirstOrDefault(x => x.IngredientId == obj.IngredientId);
                if (ingredient == null)
                {
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("Ingredients")));
                }

                if (type == EnumIngredientData.SORT_ORDER)
                {
                    obj.SortOrder = Convert.ToInt32(value);

                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == obj.ProductId && x.IngredientId == obj.IngredientId && x.SizeId == (obj.SizeId == EnumSize.REGULAR ? EnumSize.LARGE : EnumSize.REGULAR));
                    if (checkSize != null)
                    {
                        checkSize.SortOrder = obj.SortOrder;
                    }

                }
                else if (type == EnumIngredientData.QTY)
                {
                    obj.Value = value;
                    obj.Price = 0;
                    var unit = _db.Units.FirstOrDefault(x => x.UnitId == obj.UnitId);
                    if (unit != null)
                    {
                        obj.Unit = unit.Name;
                        var unitBase = _db.Units.FirstOrDefault(x => x.UnitGroupId == unit.UnitGroupId && x.IsDefault == true);
                        var percent = (double)(unit.Rate / unitBase.Rate);
                        var price = obj.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);
                        obj.Price = price;
                    }
                }
                else if (type == EnumIngredientData.UNIT)
                {
                    var unit = _db.Units.FirstOrDefault(x => x.UnitId == value);
                    if (unit != null)
                    {
                        obj.Price = 0;
                        obj.UnitId = Convert.ToInt32(value);
                        obj.Unit = unit.Name;
                        var unitBase = _db.Units.FirstOrDefault(x => x.UnitGroupId == unit.UnitGroupId && x.IsDefault == true);
                        var percent = (double)(unit.Rate / unitBase.Rate);
                        var price = obj.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);
                        obj.Price = price;
                    }
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == obj.ProductId && x.IngredientId == obj.IngredientId && x.SizeId == (obj.SizeId == EnumSize.REGULAR ? EnumSize.LARGE : EnumSize.REGULAR));
                    if (checkSize != null)
                    {
                        checkSize.UnitId = obj.UnitId;
                        checkSize.Unit = obj.Unit;
                    }
                }

                if (obj.SizeId == EnumSize.REGULAR)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == obj.ProductId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.LARGE);
                    if (checkSize != null)
                    {
                        checkSize.Value = Math.Round((obj.Value * 1.5) ?? 0, 2, MidpointRounding.AwayFromZero);
                        checkSize.Price = Math.Round((obj.Price * 1.5) ?? 0, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        var sizeLarge = new ProductIngredient();
                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                        sizeLarge.ProductId = obj.ProductId;
                        sizeLarge.SizeId = EnumSize.REGULAR;
                        sizeLarge.IngredientId = obj.IngredientId;
                        sizeLarge.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        sizeLarge.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                        sizeLarge.StatusID = obj.StatusID;
                        sizeLarge.CreateDate = obj.CreateDate;
                        sizeLarge.UnitId = obj.UnitId;
                        sizeLarge.Unit = obj.Unit;
                        sizeLarge.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeLarge.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeLarge);
                    }
                }
                else if (obj.SizeId == EnumSize.LARGE)
                {
                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == obj.ProductId && x.IngredientId == obj.IngredientId && x.SizeId == EnumSize.REGULAR);
                    if (checkSize != null)
                    {
                        checkSize.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        checkSize.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                    }
                    else
                    {
                        var sizeRegular = new ProductIngredient();
                        sizeRegular.ProductIngredientId = Guid.NewGuid();
                        sizeRegular.ProductId = obj.ProductId;
                        sizeRegular.SizeId = EnumSize.REGULAR;
                        sizeRegular.IngredientId = obj.IngredientId;
                        sizeRegular.Value = Math.Round((obj.Value * 2 / 3) ?? 0, 2);
                        sizeRegular.Price = Math.Round((obj.Price * 2 / 3) ?? 0, 2);
                        sizeRegular.StatusID = obj.StatusID;
                        sizeRegular.CreateDate = obj.CreateDate;
                        sizeRegular.UnitId = obj.UnitId;
                        sizeRegular.Unit = obj.Unit;
                        sizeRegular.ProductIngredientGroupId = obj.ProductIngredientGroupId;
                        sizeRegular.SortOrder = obj.SortOrder;
                        _db.ProductIngredients.Add(sizeRegular);
                    }
                }
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }
        }

        [Route("recipe/delete-ingredient")]
        public ActionResult DeleteIngredient(Guid id)
        {
            var productIngredients = _db.ProductIngredients.FirstOrDefault(x => x.ProductIngredientId == id);
            if (productIngredients == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));

            if (productIngredients.SizeId == EnumSize.REGULAR)
            {
                var large = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == productIngredients.ProductId && x.IngredientId == productIngredients.IngredientId && x.SizeId == EnumSize.LARGE);
                if(large != null)
                {
                    _db.ProductIngredients.Remove(large);
                }
            }
            else if (productIngredients.SizeId == EnumSize.LARGE)
            {
                var regular = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == productIngredients.ProductId && x.IngredientId == productIngredients.IngredientId && x.SizeId == EnumSize.REGULAR);
                if (regular != null)
                {
                    _db.ProductIngredients.Remove(regular);
                }
            }

            _db.ProductIngredients.Remove(productIngredients);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("recipe/get-unit-by-ingredient")]
        public string GetUnitByIngredient(int? id = null)
        {
            string cbxDataID = "";
            cbxDataID = "<select class=\"ctr-select\" data-live-search=\"true\" name=\"UnitId\" id=\"UnitId\" >";
            var ingredient = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (ingredient != null)
            {

                var lstUnits = _db.Units.Where(x => x.UnitGroupId == ingredient.UnitGroupId).OrderBy(x => x.SortOrder).ToList();
                foreach (var item in lstUnits)
                    cbxDataID += string.Format("<option value=\"{0}\" >{1}</option>", item.UnitId, item.Name);
            }
            cbxDataID += "</select>";
            return cbxDataID;
        }
        #endregion


        #region Add Ingredient group to Recipe
        [Route("recipe/update-ingredient-group")]
        public ActionResult UpdateIngredientGroup(Guid? id = null, Guid? productId = null)
        {

            var new_record = new ProductIngredientGroup();
            new_record.ProductId = productId;

            var obj = _db.ProductIngredientGroup.FirstOrDefault(x => x.ProductIngredientGroupId == id);
            obj = obj == null ? new_record : obj;

            return PartialView(obj);
        }

        [Route("recipe/update-ingredient-group")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateIngredientGroup(ProductIngredientGroup obj)
        {
            var productIngredientGroup = _db.ProductIngredientGroup.FirstOrDefault(x => x.ProductId == obj.ProductId && x.Name.ToLower() == obj.Name.ToLower().Trim());
            if (productIngredientGroup != null)
            {
                return Json(new CxResponse("err", Message.MSG_USED.Params(productIngredientGroup.Name)));
            }

            if (obj.ProductIngredientGroupId == Guid.Empty)
            {
                obj.ProductIngredientGroupId = Guid.NewGuid();
                obj.CreateDate = DateTime.Now;
                obj.StatusID = EnumStatus.ACTIVE;
                obj.SortOrder = _db.ProductIngredientGroup.Count(x => x.ProductId == obj.ProductId) + 1;
                _db.ProductIngredientGroup.Add(obj);
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                var old = _db.ProductIngredientGroup.FirstOrDefault(x => x.ProductIngredientGroupId == obj.ProductIngredientGroupId);
                if (old == null)
                    return Json(new CxResponse("err", Message.MSG_EMPTY));

                old.Name = obj.Name;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe/change-data-ingredient-group")]
        public ActionResult ChangeDataIngredientGroup(Guid? id = null, int? value = null)
        {
            var obj = _db.ProductIngredientGroup.FirstOrDefault(x => x.ProductIngredientGroupId == id);
            if (obj != null)
            {
                obj.SortOrder = value;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new CxResponse("err", Message.MSG_EXCEPTION));
            }
        }

        [Route("recipe/delete-ingredient-group")]
        public ActionResult DeleteIngredientGroup(Guid id)
        {
            var productIngredientGroup = _db.ProductIngredientGroup.FirstOrDefault(x => x.ProductIngredientGroupId == id);
            if (productIngredientGroup == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params("Header")));
            _db.ProductIngredientGroup.Remove(productIngredientGroup);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add Ingredient to Recipe by text area
        [Route("recipe/update-ingredient-text")]
        public ActionResult UpdateIngredientByText(Guid? productId = null, int? sizeId = null)
        {
            ProductIngredient productIngredient = new ProductIngredient();
            productIngredient.ProductId = productId;
            productIngredient.SizeId = sizeId;

            return PartialView(productIngredient);
        }

        [Route("recipe/update-ingredient-text")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateIngredientByText(ProductIngredient obj, Guid? ProductTempId = null, string ingredientItems = "")
        {
            string[] lines = ingredientItems.Trim().Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            var units = _db.Units.Where(x => x.StatusID != EnumStatus.DELETE);
            var ingredients = _db.Ingredients.Where(x => x.StatusID != EnumStatus.DELETE);
            var productIngredientGroups = _db.ProductIngredientGroup.Where(x => x.StatusID != EnumStatus.DELETE && x.ProductId == ProductTempId);

            List<ProductIngredient> lstProductIngredients = new List<ProductIngredient>();
            List<ProductIngredientGroup> lstProductIngredientGroups = new List<ProductIngredientGroup>();

            Guid? currentHeader = null;

            foreach (var item in lines)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (item.Contains(":"))
                    {
                        var header = item.Replace(":", "").Trim();

                        var oldHeader = productIngredientGroups.FirstOrDefault(x => x.ProductId == obj.ProductId && x.Name.ToLower() == header.ToLower());
                        if (oldHeader == null)
                        {
                            ProductIngredientGroup productIngredientGroup = new ProductIngredientGroup();
                            productIngredientGroup.ProductIngredientGroupId = Guid.NewGuid();
                            productIngredientGroup.ProductId = ProductTempId;
                            productIngredientGroup.Name = header;
                            productIngredientGroup.StatusID = EnumStatus.ACTIVE;
                            productIngredientGroup.SortOrder = productIngredientGroups.Count() + lstProductIngredientGroups.Count() + 1;
                            productIngredientGroup.CreateDate = DateTime.Now;
                            lstProductIngredientGroups.Add(productIngredientGroup);
                            currentHeader = productIngredientGroup.ProductIngredientGroupId;
                        }
                        else
                        {
                            currentHeader = oldHeader.ProductIngredientGroupId;
                        }
                    }
                    else
                    {
                        var itemInLine = item.Trim().Split(new string[] { " " }, StringSplitOptions.None);
                        var qty = itemInLine[0];
                        var unitText = itemInLine[1];
                        var ingredientText = item.Replace(string.Format("{0} {1}", qty, unitText), "").ToLower().Trim();

                        var ingredient = ingredients.FirstOrDefault(x => x.Name.ToLower().Equals(ingredientText.ToLower()));
                        if (ingredient != null)
                        {
                            var unit = units.FirstOrDefault(x => x.UnitGroupId == ingredient.UnitGroupId && x.Name.ToLower().Equals(unitText.ToLower().Trim()) || x.Code.ToLower().Equals(unitText.ToLower().Trim()));
                            if (unit != null)
                            {
                                var checkExist = _db.ProductIngredients.Count(x => x.ProductId == ProductTempId && x.IngredientId == ingredient.IngredientId);
                                if (checkExist == 0)
                                {
                                    ProductIngredient productIngredient = new ProductIngredient();
                                    productIngredient.ProductIngredientId = Guid.NewGuid();
                                    productIngredient.ProductId = ProductTempId;
                                    productIngredient.ProductIngredientGroupId = currentHeader;
                                    productIngredient.SizeId = obj.SizeId;
                                    productIngredient.IngredientId = ingredient.IngredientId;
                                    productIngredient.Value = Convert.ToDouble(qty);
                                    productIngredient.Unit = unit.Name;
                                    productIngredient.UnitId = unit.UnitId;
                                    productIngredient.StatusID = EnumStatus.ACTIVE;
                                    productIngredient.CreateDate = DateTime.Now;
                                    productIngredient.Price = 0;

                                    var unitBase = 1;
                                    var percent = (double)(unit.Rate / unitBase);
                                    var price = productIngredient.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);

                                    productIngredient.Price = price;
                                    lstProductIngredients.Add(productIngredient);

                                    var checkSize = _db.ProductIngredients.FirstOrDefault(x => x.ProductId == ProductTempId && x.IngredientId == ingredient.IngredientId && x.SizeId == EnumSize.LARGE);
                                    if (obj.SizeId == EnumSize.REGULAR && checkSize == null)
                                    {
                                        var sizeLarge = new ProductIngredient();
                                        sizeLarge.ProductIngredientId = Guid.NewGuid();
                                        sizeLarge.ProductId = productIngredient.ProductId;
                                        sizeLarge.SizeId = EnumSize.LARGE;
                                        sizeLarge.IngredientId = productIngredient.IngredientId;
                                        sizeLarge.Value = productIngredient.Value * 1.5;
                                        sizeLarge.Price = productIngredient.Price * 1.5;
                                        sizeLarge.StatusID = productIngredient.StatusID;
                                        sizeLarge.CreateDate = productIngredient.CreateDate;
                                        sizeLarge.UnitId = productIngredient.UnitId;
                                        sizeLarge.Unit = productIngredient.Unit;
                                        sizeLarge.ProductIngredientGroupId = productIngredient.ProductIngredientGroupId;
                                        sizeLarge.SortOrder = productIngredient.SortOrder;
                                        lstProductIngredients.Add(sizeLarge);

                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (lstProductIngredients != null && lstProductIngredients.Count() > 0)
            {
                _db.ProductIngredients.AddRange(lstProductIngredients);
                _db.ProductIngredientGroup.AddRange(lstProductIngredientGroups);
                _db.SaveChanges();
            }
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
        }
        #endregion


        #region Trash
        [Route("san-pham/change-status")]
        public ActionResult Change_Status(Guid id)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            if (products.StatusID == EnumStatus.ACTIVE)
                products.StatusID = EnumStatus.INACTIVE;
            else
                products.StatusID = EnumStatus.ACTIVE;
            _db.SaveChanges();
            return Json(new CxResponse<object>(products.StatusID, Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }

        [Route("san-pham/get-by-categories")]
        public ActionResult GetProductByCategory(string id, string product, string selectId = "", string isMulti = "")
        {
            var products = _db.Products.Where(x => id.Contains(x.CategoryId.ToString()) && x.StatusID == EnumStatus.ACTIVE).ToList();
            string select = string.Format("<select class='ctr-select' data-live-search='true' name=\"{0}\" {1} >", selectId, string.IsNullOrEmpty(isMulti) ? "" : "multiple");
            string cbxProduct = "<option value=\"\">Recipe Books</option>";
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

        [Route("san-pham/ingredient")]
        public ActionResult Ingredient(Guid? productId = null, string sizeId = "", string ingredientId = "")
        {
            var product = _db.Products.FirstOrDefault(x => x.ProductId == productId);
            sizeId = sizeId.Replace("null", "");
            ingredientId = ingredientId.Replace("null", "");
            var listProductIngredient = new List<ProductIngredient>();
            if (productId != null)
            {
                listProductIngredient = _db.ProductIngredients.Where(x => x.ProductId == productId).ToList();
                foreach (var item in listProductIngredient)
                {
                    item.Price = 0;
                    var unit = _db.Units.FirstOrDefault(x => x.UnitId == item.UnitId);
                    if (unit != null)
                    {
                        var unitBase = _db.Units.FirstOrDefault(x => x.UnitGroupId == unit.UnitGroupId && x.IsDefault == true);
                        var ingredient = _db.Ingredients.FirstOrDefault(x => x.IngredientId == item.IngredientId);
                        var percent = (double)(unit.Rate / unitBase.Rate);
                        var price = item.Value * percent * (ingredient != null && ingredient.Price != null ? ingredient.Price : 0);
                        item.Price = price;
                    }
                }
            }
            var listIngredientId = ingredientId.Split(',').ToList();
            var listSizeId = sizeId.Split(',').ToList();
            var listIngredient = _db.Ingredients.Where(x => listIngredientId.Contains(x.IngredientId.ToString())).ToList();
            var listSize = _db.Sizes.Where(x => listSizeId.Contains(x.SizeId.ToString())).ToList();
            var listUnit = _db.Units.Where(x => x.StatusID == EnumStatus.ACTIVE).ToList();
            ViewBag.Size = listSize;
            ViewBag.Ingredient = listIngredient;
            ViewBag.Units = listUnit;
            return PartialView(listProductIngredient);
        }
        #endregion

    }
}