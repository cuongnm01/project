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
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            ViewBag.User = nd_dv;
            return View();
        }

        [Route("danh-muc-san-pham/list")]
        public ActionResult List(string keyword = "", int? status = EnumStatus.ACTIVE, int sotrang = 1, int tongsodong = 5)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            if (keyword != "")
                keyword = keyword.RemoveUnicode().ToLower();

            var list = (from a in _db.Categorys.ToList()
                            //where status == null ? true : a.StatusID == status
                        select a);
                        //.OrderByDescending(x => x.CreatedDate);

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
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var listCategories = _db.Categorys
                //.Where(x => x.StatusID == EnumStatus.ACTIVE)
                .ToList();
            var productCategories = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);

            string cbxSTT = "<option value=\"\">Thứ tự</option>";

            for (var i = 0; i < listCategories.Count() + 1; i++)
            {
                var index = i + 1;
                if (listCategories.Where(x => x.SortOrder == index).ToList().Count() == 0)
                {
                    cbxSTT += string.Format("<option value=\"{0}\" {2}>{1}</option>", index, "Số " + index, productCategories != null && productCategories.SortOrder == index ? "selected" : "");
                }
                else
                {
                    cbxSTT += string.Format("<option value=\"{0}\" {2}>{1}</option>", index, "Số " + index, "selected");
                }

            }
            if (productCategories != null)
            {
                if (productCategories.SortOrder == 0)
                {
                    cbxSTT += string.Format("<option value='0' selected>Không sử dụng</option>");
                }
                else
                {
                    cbxSTT += string.Format("<option value=\"{0}\" {2}>{1}</option>", productCategories.SortOrder, "Số " + productCategories.SortOrder, "selected");
                }

            }
            ViewBag.cbxSTT = cbxSTT;
            return PartialView(productCategories);
        }

        [Route("danh-muc-san-pham/update")]
        [HttpPost]
        public ActionResult Update(Category productCategory, HttpPostedFileBase _Logo = null)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });
            //var config = _db.AppConfig.FirstOrDefault();
            //config.Version = Helpers.ChangeVersion(config.Version);

            if (productCategory.CategoryId == 0)
            {
                //var checkTontai = _db.Sliders.FirstOrDefault(x => x.StatusID == EnumStatus.ACTIVE && x.STT == slider.STT);
                //if (checkTontai != null)
                //    return Json(new { kq = "err", msg = Translate.SLIDER_STT_IS_USED }, JsonRequestBehavior.AllowGet);

                // tao moi
                var stt = _db.Categorys.Count() + 1;
                productCategory.SortOrder = stt;
                if (_Logo != null)
                {
                    string rootPathImage = string.Format("~/Files/product-category/{0}", DateTime.Now.ToString("dd/MM/yyyy"));
                    string filePathImage = Path.Combine(Request.MapPath(rootPathImage));
                    string[] fileImage = _Logo.uploadFile(rootPathImage, filePathImage);
                    productCategory.Image = fileImage[1];
                }
                //productCategory.Id = Guid.NewGuid();
                //productCategory.CreatedDate = DateTime.Now;
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
                //old.de = productCategory.Descriptions;
                //old.IsCombo = productCategory.IsCombo;
                 _db.SaveChanges();

                return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_UPDATE)));
            }
        }

        [Route("danh-muc-san-pham/delete")]
        public  ActionResult Delete(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

            var category = _db.Categorys.FirstOrDefault(x => x.CategoryId == id);
            if (category == null)
                return Json(new CxResponse("err", Message.MSG_NOT_FOUND.Params(Message.F_PRODUCT_CATEGORY)));

            //category.StatusID = EnumStatus.DELETE;
            //_db.SaveChanges();
            return Json(new CxResponse(Message.MSG_SUCESS.Params(Message.ACTION_DELETE)), JsonRequestBehavior.AllowGet);
        }

        [Route("danh-muc-san-pham/change-status")]
        public  ActionResult Change_Status(int id)
        {
            var nd_dv = GetUserLogin;
            if (nd_dv == null || nd_dv.Users.PermissionID != EnumUserType.ADMIN)
                return RedirectToAction("Index", "Home", new { area = "" });

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