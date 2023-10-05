using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.DbSet;
using Project.Model.Model;
using Project.Model.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Service.Areas.Admin.Controllers
{
    [RouteArea("Admin", AreaPrefix = "admin")]
    [Route("{action}")]
    public class FavoriteController : BaseController
    {
        AppDbContext _db = new AppDbContext();

        // GET: Admin/Favorite
        #region Favorite
        [Route("favorite/main-page")]
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("favorite/list")]
        public ActionResult List(string keyword = "", int sotrang = 1, int tongsodong = 5)
        {

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Products.ToList()
                        join b in _db.Categorys.ToList() on a.CategoryId equals b.CategoryId
                        where a.StatusID == EnumStatus.ACTIVE && a.IsNew == EnumStatus.ACTIVE &&
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

        [Route("favorite/insert")]
        public ActionResult Insert()
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var product = new Product();

            var products = _db.Products.Where(x => x.StatusID == EnumStatus.ACTIVE && x.IsNew != EnumStatus.ACTIVE).ToList();
            string cbxProducts = "";
            foreach (var item in products)
            {
                cbxProducts += string.Format("<option value=\"{0}\">{1}</option>", item.ProductId, item.Name);
            }
            ViewBag.cbxProducts = cbxProducts;

            return PartialView(product);
        }

        [Route("favorite/insert")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Insert(Product product)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var obj = _db.Products.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (obj == null)
            {
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)));
            }
            else
            {
                obj.IsNew = EnumStatus.ACTIVE;
                _db.SaveChanges();
                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_INSERT)));
            }
        }


        [Route("favorite/delete")]
        public ActionResult Delete(Guid id)
        {
            CheckPermission(EnumFunctions.Recipe, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var products = _db.Products.FirstOrDefault(x => x.ProductId == id);
            if (products == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT)), JsonRequestBehavior.AllowGet);
            products.IsNew = EnumStatus.INACTIVE;
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("favorite/delete-all")]
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

            foreach(var item in products)
            {
                item.IsNew = EnumStatus.INACTIVE;
            }

            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}