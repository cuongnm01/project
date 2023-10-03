using Common.Constants;
using Common.Helpers;
using Common.Resources;
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
    public class CategoriesController : BaseController
    {
        AppDbContext _db = new AppDbContext();
        [Route("recipe-book/main-page")]
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("recipe-book/list")]
        public ActionResult List(string keyword = "", int sotrang = 1, int tongsodong = 5)
        {

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var recipes = _db.Products.Where(x => x.StatusID != EnumStatus.DELETE);

            var list = (from a in _db.Categorys.ToList()
                        where a.StatusID != EnumStatus.DELETE && (keyword == "" ? true : a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select new CategoryInfo
                        {
                            Category = a,
                            TotalRecipe = recipes.Count(x => x.CategoryId == a.CategoryId),
                        }).OrderBy(x => x.Category.Name);

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

        [Route("recipe-book/update")]
        public ActionResult Update(int? id)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new Category();
            new_record.SortOrder = _db.Categorys.Count() + 1;

            var obj = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            obj = obj == null ? new_record : obj;
            return PartialView(obj);
        }

        [Route("recipe-book/update")]
        [HttpPost]
        public ActionResult Update(Category productCategory, HttpPostedFileBase _Logo = null)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            if (productCategory.CategoryId == 0)
            {
                // tao moi
                if (_Logo != null)
                {
                    string rootPathImage = string.Format("/Files/product-category/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    productCategory.Image = fileImage[1];
                }
                productCategory.CreateDate = DateTime.Now;

                if(productCategory.IsHomePage == EnumStatus.ACTIVE)
                {
                    var home = _db.Categorys.FirstOrDefault(x => x.IsHomePage == EnumStatus.ACTIVE);
                    if(home != null)
                    {
                        home.IsHomePage = EnumStatus.INACTIVE;
                    }
                }

                _db.Categorys.Add(productCategory);
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
            else
            {
                // up date
                var old = _db.Categorys.FirstOrDefault(x => x.CategoryId == productCategory.CategoryId);
                if (old == null)
                {
                    return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));
                }
                if (_Logo != null)
                {
                    string rootPathImage = string.Format("~/Files/product-category/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    old.Image = fileImage[1];
                }

                if (productCategory.IsHomePage == EnumStatus.ACTIVE)
                {
                    var home = _db.Categorys.FirstOrDefault(x => x.IsHomePage == EnumStatus.ACTIVE);
                    if (home != null)
                    {
                        home.IsHomePage = EnumStatus.INACTIVE;
                    }
                }

                old.Name = productCategory.Name;
                old.StatusID = productCategory.StatusID;
                old.IsHomePage = productCategory.IsHomePage;

                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("recipe-book/delete")]
        public ActionResult Delete(int id)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            category.StatusID = EnumStatus.DELETE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("recipe-book/delete-all")]
        public ActionResult DeleteAll(string ids)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var cateIds = ids.Split(',').ToList();
            var category = _db.Categorys.Where(x => cateIds.Contains(x.CategoryId.ToString()));
            if (category == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            _db.Categorys.RemoveRange(category);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

    }
}