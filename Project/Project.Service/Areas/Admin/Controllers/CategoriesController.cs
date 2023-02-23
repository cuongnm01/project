using Common.Constants;
using Common.Helpers;
using Common.Resources;
using Project.Model;
using Project.Model.DbSet;
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
        [Route("danh-muc-san-pham/main-page")]
        public ActionResult MainPage()
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.VIEW);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            ViewBag.User = nd_dv;
            return View();
        }

        [Route("danh-muc-san-pham/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            
            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Categorys.ToList()
                        where keyword == "" || (keyword != "" && a.Name.RemoveUnicode().ToLower().Contains(keyword))
                        select a);

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

        [Route("danh-muc-san-pham/update")]
        public ActionResult Update(int? id)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var new_record = new Category();
            new_record.SortOrder = _db.Categorys.Count() + 1;

            var obj = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            obj = obj == null ? new_record : obj;
            return PartialView(obj);
        }

        [Route("danh-muc-san-pham/update")]
        [HttpPost]
        public ActionResult Update(Category productCategory, HttpPostedFileBase _Logo = null)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
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
                productCategory.StatusID = EnumStatus.ACTIVE;
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
                old.Name = productCategory.Name;
                old.SortOrder = productCategory.SortOrder;
                _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("danh-muc-san-pham/delete")]
        public ActionResult Delete(int id)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.DELETE);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            category.StatusID = EnumStatus.DELETE;
            _db.Categorys.Remove(category);
            _db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("danh-muc-san-pham/change-status")]
        public ActionResult Change_Status(int id)
        {
            CheckPermission(EnumFunctions.Recipe_Book, EnumOptions.ADD);
            var nd_dv = GetUserLogin;
            if (nd_dv.AccessDenied == EnumStatus.ACTIVE)
                return RedirectToAction("AccessDenied", "Home", new { area = "" });

            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            //if (category.StatusID == EnumStatus.ACTIVE)
            //    category.StatusID = EnumStatus.INACTIVE;
            //else
            //    category.StatusID = EnumStatus.ACTIVE;
            //_db.SaveChanges();
            return Json(new CxResponse<object>(/*category.StatusID, */Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)), JsonRequestBehavior.AllowGet);
        }


    }
}