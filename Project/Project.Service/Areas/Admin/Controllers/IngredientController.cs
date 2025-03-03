using Common.Constants;
using Project.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Helpers;
using Project.Model.DbSet;
using System.IO;
using Project.Model.Respone;
using Common.Resources;
using Project.Model.Model;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class IngredientController : BaseController
    {
        // GET: Admin/Ingredient
        AppDbContext _db = new AppDbContext();

        #region ingredient group
        [Route("ingredient-group/main-page")]
        public ActionResult MainPage_Group()
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("ingredient-group/list")]
        public ActionResult List_Group(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var ingredients = _db.Ingredients;

            var list = (from a in _db.IngredientGroups.ToList()
                        where (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new IngredientGroupInfo
                        {
                            IngredientGroup = a,
                            TotalIngredient = ingredients.Count(x => x.IngredientGroupId == a.IngredientGroupId),
                        })
            .OrderBy(x => x.IngredientGroup.Name);

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

        [Route("ingredient-group/update")]
        public ActionResult Update_Group(int? id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var listIngredients = _db.IngredientGroups.ToList();
            var slider = _db.IngredientGroups.FirstOrDefault(x => x.IngredientGroupId == id);
            return PartialView(slider);
        }

        [Route("ingredient-group/update")]
        [HttpPost]
        public ActionResult Update_Group(IngredientGroup obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.IngredientGroupId == 0)
                {
                    obj.CreateDate = DateTime.Now;
                    _db.IngredientGroups.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.IngredientGroups.FirstOrDefault(x => x.IngredientGroupId == obj.IngredientGroupId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }

                    old.Name = obj.Name;
                    old.SortOrder = obj.SortOrder;
                    old.StatusID = obj.StatusID;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("ingredient-group/delete")]
        public ActionResult Delete_Group(int id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.IngredientGroups.FirstOrDefault(x => x.IngredientGroupId == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            //obj.StatusID = EnumStatus.DELETE;

            _db.IngredientGroups.Remove(obj);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("ingredient-group/delete-all")]
        public ActionResult Delete_GroupAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var ingreIds = ids.Split(',').ToList();
            var ingredients = _db.IngredientGroups.Where(x => ingreIds.Contains(x.IngredientGroupId.ToString()));
            if (ingredients == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            _db.IngredientGroups.RemoveRange(ingredients);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ingredient
        [Route("ingredient/main-page")]
        public ActionResult MainPage(int? unitGroupId = null, int? ingredientGroupId = null)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            ViewBag.unitGroupId = unitGroupId ?? 0;
            ViewBag.ingredientGroupId = ingredientGroupId ?? 0;

            return View();
        }

        [Route("ingredient/list")]
        public ActionResult List(int? unitGroupId = null, int? ingredientGroupId = null, string keyword = "", int sotrang = 1, int tongsodong = 5)
        {
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Ingredients.ToList()
                        join b in _db.IngredientGroups on a.IngredientGroupId equals b.IngredientGroupId
                        join c in _db.UnitGroups on a.UnitGroupId equals c.UnitGroupId
                        where (unitGroupId == null || unitGroupId == 0 ? true : a.UnitGroupId == unitGroupId)
                        && (ingredientGroupId == null || ingredientGroupId == 0 ? true : a.IngredientGroupId == ingredientGroupId)
                        && a.StatusID != EnumStatus.DELETE
                         && (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new IngredientInfo
                        {
                            Ingredient = a,
                            IngredientGroup = b,
                            UnitGroup = c,
                        })
            .OrderBy(x => x.IngredientGroup.Name).ThenBy(x => x.Ingredient.Name);

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

        [Route("ingredient/update")]
        public ActionResult Update(int? id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new Ingredient();
            var sortOrder = _db.Ingredients.OrderByDescending(x => x.SortOrder).Select(x => x.SortOrder).FirstOrDefault();
            new_record.SortOrder = sortOrder != null ? (sortOrder + 1) : 1;

            var obj = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            obj = obj == null ? new_record : obj;

            var category = _db.IngredientGroups
             .Where(x => x.StatusID == EnumStatus.ACTIVE)
             .ToList();
            string cbxCategory = "";
            foreach (var item in category)
            {
                cbxCategory += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.IngredientGroupId, item.Name, obj != null && obj.IngredientGroupId == item.IngredientGroupId ? "selected" : "");
            }
            ViewBag.cbxCategory = cbxCategory;

            var categoryUnit = _db.UnitGroups
             .Where(x => x.StatusID == EnumStatus.ACTIVE)
             .ToList();
            string cbxCategoryUnit = "";
            foreach (var item in categoryUnit)
            {
                cbxCategoryUnit += string.Format("<option value=\"{0}\" {2}>{1}</option>", item.UnitGroupId, item.Name, obj != null && obj.UnitGroupId == item.UnitGroupId ? "selected" : "");
            }
            ViewBag.cbxCategoryUnit = cbxCategoryUnit;

            return PartialView(obj);
        }

        [Route("ingredient/update")]
        [HttpPost]
        public ActionResult Update(Ingredient obj, HttpPostedFileBase _Logo = null)
        {
            try
            {
                CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
                var nd_dv = GetUserLogin;
                if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                    return RedirectToAction("AccessDenied", "Home", new { area = "" });

                if (obj.IngredientId == 0)
                {
                    var exists = _db.Ingredients.Count(x => x.Name.ToLower().Trim() == obj.Name.ToLower().Trim());
                    if(exists > 0)
                        return Json(new CxResponse("err", "Ingredients name existed"));

                    // tao moi
                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        obj.Image = fileImage[1];
                    }
                    obj.CreateDate = DateTime.Now;
                    _db.Ingredients.Add(obj);
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
                }
                else
                {
                    // up date
                    var old = _db.Ingredients.FirstOrDefault(x => x.IngredientId == obj.IngredientId);
                    if (old == null)
                    {
                        return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
                    }

                    var exists = _db.Ingredients.Count(x => x.IngredientId != obj.IngredientId && x.Name.ToLower().Trim() == obj.Name.ToLower().Trim());
                    if (exists > 0)
                        return Json(new CxResponse("err", "Ingredients name existed"));

                    if (_Logo != null)
                    {
                        string rootPathImage = string.Format("~/Files/slider/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                        string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                        string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                        old.Image = fileImage[1];
                    }
                    old.IngredientGroupId = obj.IngredientGroupId;
                    old.Name = obj.Name;
                    old.UnitGroupId = obj.UnitGroupId;
                    old.Price = obj.Price;
                    old.StatusID = obj.StatusID;
                    old.CreateDate = DateTime.Now;
                    _db.SaveChanges();

                    return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
                }

            }
            catch (Exception e)
            {
                return Json(new CxResponse(Message.MSG_EXCEPTION));
            }

        }

        [Route("ingredient/delete")]
        public ActionResult Delete(int id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));
            obj.StatusID = EnumStatus.DELETE;
            _db.Ingredients.Remove(obj);

            var productIngredients = _db.ProductIngredients.Where(x => x.IngredientId == id);
            if (productIngredients != null && productIngredients.Count() > 0)
                _db.ProductIngredients.RemoveRange(productIngredients);

            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("ingredient/delete-all")]
        public ActionResult DeleteAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var ingreIds = ids.Split(',').ToList();

            var ingredients = _db.Ingredients.Where(x => ingreIds.Contains(x.IngredientId.ToString()));
            if (ingredients == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            _db.Ingredients.RemoveRange(ingredients);

            var productIngredients = _db.ProductIngredients.Where(x => ingreIds.Contains(x.IngredientId.ToString()));
            if (productIngredients != null && productIngredients.Count() > 0)
                _db.ProductIngredients.RemoveRange(productIngredients);

            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("ingredient/change-status")]
        public ActionResult Change_Status(int id)
        {
            CheckPermission(EnumFunctions.Ingredient, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (obj == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_SLIDER)));

            if (obj.StatusID == EnumStatus.ACTIVE)
                obj.StatusID = EnumStatus.INACTIVE;
            else
                obj.StatusID = EnumStatus.ACTIVE;

            _db.SaveChanges();
            return Json(new CxResponse<object>(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }

        [Route("ingredient/delete-image")]
        public ActionResult DeleteImage(int id, int? type = null)
        {
            var ingredients = _db.Ingredients.FirstOrDefault(x => x.IngredientId == id);
            if (ingredients == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);

            ingredients.Image = null;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}